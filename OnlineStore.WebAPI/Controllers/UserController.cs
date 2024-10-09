using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;

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

        [Authorize("Admin")]
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAll()
        {
            IEnumerable<User?> users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto(u));
        }

        [Authorize("Admin")]
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<UserDto?> GetById(int id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            return new UserDto(user);
        }
    }
}
