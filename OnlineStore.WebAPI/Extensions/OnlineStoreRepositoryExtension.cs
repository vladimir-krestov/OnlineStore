using OnlineStore.Core.Interfaces;
using OnlineStore.Infrastructure.Repositories;

namespace OnlineStore.WebAPI.Extensions
{
    public static class OnlineStoreRepositoryExtension
    {
        public static void AddStoreRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPizzaRepository, PizzaRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
