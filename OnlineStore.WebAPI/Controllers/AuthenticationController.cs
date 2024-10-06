using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.Core.Models;
using OnlineStore.WebAPI.Attributes;
using OnlineStore.WebAPI.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //[HttpPost]
        //public string SignUp([FromBody]AuthenticationRequest request)
        //{
        //    return "User created";
        //}

        [HttpPost("SignIn", Name = "SignIn")]
        //[Route("SignIn")]
        public Task<string> SignIn([FromBody] AuthenticationRequest request)
        {
            // Check email and pass

            // Provide a JWT token
            return Task.FromResult(GenerateJwtToken(request.Email));
        }

        [CustomAuthorization(UserRole.Admin)]
        [HttpGet(Name = "GenerateKey")]
        public string GenerateKey()
        {
            byte[] salt = new byte[32];
            using RandomNumberGenerator cryptoProvider = RandomNumberGenerator.Create();
            cryptoProvider.GetBytes(salt);

            return Convert.ToBase64String(salt);
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
