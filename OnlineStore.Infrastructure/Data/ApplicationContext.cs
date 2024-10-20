using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Models;
using System.Diagnostics;

namespace OnlineStore.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Debug.WriteLine("Created ApplicationContext");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderNumber)
                .HasPrincipalKey(o => o.Number);

            modelBuilder.Entity<UserRoleMapping>()
                .HasKey(urm => new { urm.UserId, urm.RoleId });

            modelBuilder.Entity<Log>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(log => log.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoleMapping>()
                .HasOne(urm => urm.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(urm => urm.UserId);

            modelBuilder.Entity<UserRoleMapping>()
                .HasOne(urm => urm.Role)
                .WithMany()
                .HasForeignKey(urm => urm.RoleId);
        }
    }
}
