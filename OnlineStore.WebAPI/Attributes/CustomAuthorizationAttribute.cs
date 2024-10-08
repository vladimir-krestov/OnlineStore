﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.Core.Models;
using OnlineStore.Infrastructure.Data;
using OnlineStore.WebAPI.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineStore.WebAPI.Attributes
{
    [Obsolete("This attribute was for studying purposes only. Now it is replaces with build-in services.")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CustomAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _requiredRoles;

        public CustomAuthorizationAttribute()
        {
        }

        public CustomAuthorizationAttribute(params string[] userRoles)
        {
            _requiredRoles = userRoles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string? token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Check authentication
            if (token == null || !ValidateToken(token, context))
            {
                context.Result = new UnauthorizedResult();
            }

            // Check autorization
            if (_requiredRoles is null || _requiredRoles.Contains(string.Empty))
            {
                // This instance of the attribute is available for any role, so it's not necessary to check the user role stored in the database.
                return;
            }

            // Take the user email from the token claims
            ClaimsPrincipal userPrincipal = context.HttpContext.User;
            string? userEmail = userPrincipal.FindFirst(ClaimTypes.Email)?.Value;

            // Search a user with this email in the database
            ApplicationContext applicationContext = context.HttpContext.RequestServices.GetService<ApplicationContext>() ?? throw new InvalidOperationException("ApplicationContext is not set correctly to Services.");
            User? user = await applicationContext.Users
                .Include(u => u.Roles)
                .ThenInclude(urm => urm.Role)
                .FirstOrDefaultAsync(user => user.Email == userEmail);

            if (user is null || _requiredRoles.Any(requredRole => user.Roles.Any(r => r.Role.Name == requredRole)))
            {
                context.Result = new ForbidResult();
                return;
            }

            context.HttpContext.User.AddIdentity(new ClaimsIdentity([new Claim("UserId", user.Id.ToString())]));
        }

        private bool ValidateToken(string token, AuthorizationFilterContext context)
        {
            string? jwtKey = ConfigurationHelper.JwtKey;
            string? jwtIssuer = ConfigurationHelper.GetConfigValue("Jwt:Issuer");
            string? jwtAudience = ConfigurationHelper.GetConfigValue("Jwt:Audience");

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                // to-do: Log it
                // throw new InvalidOperationException($"Missconfiguration of {nameof(jwtKey)} or {nameof(jwtIssuer)} or {nameof(jwtAudience)}.");

                return false;
            }

            try
            {
                JwtSecurityTokenHandler tokenHandler = new();
                byte[] key = Encoding.UTF8.GetBytes(jwtKey);
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // Избегаем погрешностей во времени
                }, out SecurityToken validatedToken);

                context.HttpContext.User = principal;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
