using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IPizzaRepository : IRepository<Pizza>
    {
        Task<Pizza?> GetByIdAsync(int id);
    }
}
