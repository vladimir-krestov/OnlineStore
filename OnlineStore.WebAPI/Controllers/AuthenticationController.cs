using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.Core.Models;
using OnlineStore.WebAPI.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Repositories;
using OnlineStore.Core.Models.Dto;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthenticationController(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register([FromBody] RegistrationRequest request)
        {
            // to-do: Validation of request data
            User? user = await _userRepository.RegisterNewUserAsync(request);
            if (user is not null)
            {
                return Ok();
            }

            return BadRequest("The user can't be registrated.");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult> Login([FromBody] AuthenticationRequest request)
        {
            // Check email and pass
            User? user = await _userRepository.GetUserByLoginAndPassAsync(request);
            if (user is null)
            {
                return BadRequest("User hasn't been found.");
            }

            // User is authenticated, generate cookies
            await GenerateCookie(user);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route(nameof(GenerateKey))]
        public string GenerateKey()
        {
            byte[] salt = new byte[32];
            using RandomNumberGenerator cryptoProvider = RandomNumberGenerator.Create();
            cryptoProvider.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("AddUserRole")]
        public async Task<ActionResult<bool>> AddUserRole([FromQuery] string userId, [FromQuery] string userRoleName)
        {
            try
            {
                return Ok(await _userRepository.AddUserRoleAsync(userId, userRoleName));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("RemoveUserRole")]
        public async Task<ActionResult<bool>> RemoveUserRole([FromQuery] string userId, [FromQuery] string userRoleName)
        {
            try
            {
                return Ok(await _userRepository.RemoveUserRoleAsync(userId, userRoleName));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("AccessDenied")]
        public ActionResult AccessDenied()
        {
            return Forbid();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Unauthorize")]
        public ActionResult Unauthorize()
        {
            return Unauthorized();
        }

        private async Task GenerateCookie(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (UserRoleMapping userRole in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false, // Cookie сохраняется после закрытия браузера
                ExpiresUtc = DateTime.UtcNow.AddMinutes(2)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        private string GenerateJwtToken(string email)
        {
            string? jwtKey = ConfigurationHelper.JwtKey;
            string? jwtIssuer = _configuration["Jwt:Issuer"];
            string? jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException($"Missconfiguration of {nameof(jwtKey)} or {nameof(jwtIssuer)} or {nameof(jwtAudience)}.");
            }

            Claim[] claims = [
                    new Claim(ClaimTypes.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ];

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
