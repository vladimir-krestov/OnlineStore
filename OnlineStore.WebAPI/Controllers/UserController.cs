using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;
using OnlineStore.Core.Services;
using OnlineStore.WebAPI.Attributes;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<PizzaController> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IUserManager _userManager;

        public UserController(ILogger<PizzaController> logger, IRepository<User> userRepository, IUserManager userManager)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [CustomAuthorization(UserRole.Admin)]
        [HttpGet(Name = "GetAllUsers")]
        public async Task<IEnumerable<UserDto>> GetAll()
        {
            IEnumerable<User?> users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto(u));
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<UserDto?> GetById(int id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            return new UserDto(user);
        }

        [HttpPost(Name = "CreateNewUser")]
        public async Task<UserDto?> CreateNewUser([FromBody] UserDto userDto, [FromQuery] string password)
        {
            User? user = await _userRepository.CreateNewAsync(_userManager.CreateUser(userDto, password));

            return new UserDto(user);
        }
    }
}
