using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByLoginAndPassAsync(AuthenticationRequest request);

        Task<User?> RegisterNewUserAsync(RegistrationRequest request);

        Task<bool> AddUserRoleAsync(string userId, string userRoleName);

        Task<bool> RemoveUserRoleAsync(string userId, string userRoleName);
    }
}
