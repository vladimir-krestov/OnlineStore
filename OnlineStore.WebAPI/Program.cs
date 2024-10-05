using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Services;
using OnlineStore.Infrastructure.Data;
using OnlineStore.Infrastructure.Repositories;

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

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IRepository<Pizza>>(provider => new Repository<Pizza>(provider.GetService<ApplicationContext>()));
            builder.Services.AddScoped<IRepository<User>>(provider => new Repository<User>(provider.GetService<ApplicationContext>()));
            builder.Services.AddScoped<IRepository<Order>>(provider => new Repository<Order>(provider.GetService<ApplicationContext>()));
            builder.Services.AddScoped<IUserManager, UserManager>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();

            app.Run();
        }
    }
}
