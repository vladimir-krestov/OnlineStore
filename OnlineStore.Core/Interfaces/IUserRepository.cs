using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByIdAsync(int id);
    }
}
