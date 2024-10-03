using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Models;

namespace OnlineStore.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasKey(p => p.Id);
        }
    }
}
