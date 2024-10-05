using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;

namespace OnlineStore.Core.Services
{
    public class UserManager : IUserManager
    {
        public User CreateUser(UserDto userDto, string password)
        {
            string salt = PasswordManager.GenerateSalt();
            string hash = PasswordManager.HashPassword(password, salt);

            User user = new()
            {
                Email = userDto.Email,
                Name = userDto.Name,
                Address = userDto.Address,
                PhoneNumber = userDto.PhoneNumber,

                PasswordHash = hash,
                Salt = salt,
                CreationDate = DateTime.UtcNow
            };

            return user;
        }
    }
}
