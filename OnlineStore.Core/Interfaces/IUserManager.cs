using OnlineStore.Core.Models.Dto;
using OnlineStore.Core.Models;

namespace OnlineStore.Core.Interfaces
{
    public interface IUserManager
    {
        User CreateUser(UserDto userDto, string password);
    }
}
