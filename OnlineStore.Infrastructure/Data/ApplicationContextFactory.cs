using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OnlineStore.Core.Interfaces;

namespace OnlineStore.Infrastructure.Data
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            // todo: move to config:
            optionsBuilder.UseSqlServer("");

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
