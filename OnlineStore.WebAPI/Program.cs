using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Services;
using OnlineStore.Infrastructure.Data;
using OnlineStore.WebAPI.Extensions;
using OnlineStore.WebAPI.Middlewares;
using OnlineStore.WebAPI.Utilities;
using System.Diagnostics;
using System.Text;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using OnlineStore.Core.Models.Dto;

namespace OnlineStore.WebAPI
{
    public class Program
    {
        // EF commands:
        // dotnet ef migrations add <Name>
        // dotnet ef database update

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/Properties/NLog.config"));
            LogManager.Configuration.Variables["connectionString"] = builder.Configuration.GetConnectionString("DefaultConnection");
            ConfigurationHelper.Configuration = builder.Configuration;

            string? authenticationMethod = builder.Configuration.GetValue<string>("AuthenticationMethod");
            if (authenticationMethod == "Jwt")
            {
                ConfigurationHelper.JwtKey = Environment.GetEnvironmentVariable("OnlineStore_JWT_Key");
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.JwtKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            }
            else if (authenticationMethod == "Cookies")
            {
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Unauthorize";
                    options.AccessDeniedPath = "/AccessDenied";
                    options.Cookie.Name = "AuthCookie";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(2);
                    options.SlidingExpiration = true;
                });
            }
            else
            {
                throw new NotSupportedException($"This authentication method: {authenticationMethod} is not supported. Check the config file.");
            }

            // Add services to the container.
            builder.Services.AddHealthChecks()
                .AddAsyncCheck("example_check", () =>
                    Task.FromResult(HealthCheckResult.Healthy("The check indicates a healthy state.")))
                .AddDbContextCheck<ApplicationContext>("Database", HealthStatus.Unhealthy)
                .AddCheck<TimeHealthCheck>("TimeCheck");
            ;
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(ConfigurationHelper.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ILoggerManager, LoggerManager>();

            builder.Services.AddStoreRepositories(); // custom
            builder.Services.AddScoped<IUserManager, UserManager>();
            builder.Services.AddSingleton(new SemaphoreSlim(90));
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            if (authenticationMethod == "Jwt")
            {
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Input JWT token: Bearer {token}",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                    });
                });
            }
            else
            {
                builder.Services.AddSwaggerGen();
            }

            builder.Services.AddLogging();
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<TypedMemoryCache<int, UserDto>>();
            builder.Services.AddSingleton<FactorialBackgroundService>();
            builder.Services.AddHostedService(provider => provider.GetRequiredService<FactorialBackgroundService>());

            var app = builder.Build();

            app.Use(async (ctx, next) =>
            {
                await next.Invoke(ctx);

                Debug.WriteLine("Inside the custom lambda middleware after next.");

                if (ctx.Response.StatusCode == 404)
                {
                    ctx.Response.StatusCode = 403;
                }
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRequestPageCheck(); // custom
            app.UseRequestId(); // custom

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.MapHealthChecks("/healthcheck", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new {
                            component = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description ?? "No description",
                            duration = e.Value.Duration.TotalMilliseconds + " ms"
                        }),
                        totalDuration = report.TotalDuration.TotalMilliseconds + " ms"
                    });

                    await context.Response.WriteAsync(result);
                }
            });
            app.MapControllers();
            app.UseRequestIdCheck(); // custom

            app.Run();
        }
    }
}
