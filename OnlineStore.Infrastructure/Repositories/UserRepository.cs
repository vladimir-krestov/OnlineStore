using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;
using OnlineStore.Core.Services;
using OnlineStore.Infrastructure.Data;
using System.Data;

namespace OnlineStore.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByLoginAndPassAsync(AuthenticationRequest request)
        {
            // Identify the user, check if the user exists at all
            User? user = await _context.Users
                .Include(u => u.Roles)
                .ThenInclude(urm => urm.Role)
                .FirstOrDefaultAsync(user => user.Email == request.Email);

            if (user is null)
            {
                return null;
            }

            // Authenticate the user, check the user's password
            string hash = PasswordManager.HashPassword(request.Password, user.Salt);
            if (hash == user.PasswordHash)
            {
                return user;
            }

            return null;
        }

        public async Task<User?> RegisterNewUserAsync(RegistrationRequest request)
        {
            const string DefaultRole = "Customer";

            string salt = PasswordManager.GenerateSalt();
            string hash = PasswordManager.HashPassword(request.Password, salt);
            UserRole userRole = await _context.UserRoles.FirstOrDefaultAsync(role => role.Name == DefaultRole)
                ?? throw new InvalidOperationException($"Role: {DefaultRole} hasn't been found.");

            User user = new()
            {
                // Personal data
                Email = request.Email,
                Name = request.Name,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,

                // System data
                PasswordHash = hash,
                Salt = salt,
                CreationDate = DateTime.UtcNow
            };

            user.Roles = new List<UserRoleMapping>() { new UserRoleMapping { Role = userRole, User = user } };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> AddUserRoleAsync(string userId, string userRoleName)
        {
            if (!int.TryParse(userId, out int id))
            {
                throw new InvalidOperationException($"User id: {userId} can't be parsed to int.");
            }

            UserRole userRole = await _context.UserRoles.FirstOrDefaultAsync(role => role.Name == userRoleName)
                ?? throw new InvalidOperationException($"Role: {userRoleName} hasn't been found.");

            User user = await _context.Users
                .Include(u => u.Roles)
                .ThenInclude(urm => urm.Role)
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new InvalidOperationException($"User with id: {id} hasn't been found.");

            // Check if the role is already set
            if (user.Roles.Any(urm => urm.Role.Name == userRole.Name))
            {
                // The new role hasn't been added
                return false;
            }

            user.Roles.Add(new UserRoleMapping { Role = userRole, User = user });
            await _context.SaveChangesAsync();

            // The new role has been added
            return true;
        }

        public async Task<bool> RemoveUserRoleAsync(string userId, string userRoleName)
        {
            if (!int.TryParse(userId, out int id))
            {
                throw new InvalidOperationException($"User id: {userId} can't be parsed to int.");
            }

            UserRole userRole = await _context.UserRoles.FirstOrDefaultAsync(role => role.Name == userRoleName)
                ?? throw new InvalidOperationException($"Role: {userRoleName} hasn't been found.");

            User user = await _context.Users
                .Include(u => u.Roles)
                .ThenInclude(urm => urm.Role)
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new InvalidOperationException($"User with id: {id} hasn't been found.");

            // Check if the role is set
            UserRoleMapping? mapping = user.Roles.FirstOrDefault(urm => urm.Role.Name == userRole.Name);
            if (mapping is null)
            {
                // The role hasn't been added
                return false;
            }

            user.Roles.Remove(mapping);
            await _context.SaveChangesAsync();

            // The role has been removed
            return true;
        }
    }
}
