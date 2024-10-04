using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Models;

namespace OnlineStore.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Pizza>().HasKey(p => p.Id);
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
        }
    }
}
