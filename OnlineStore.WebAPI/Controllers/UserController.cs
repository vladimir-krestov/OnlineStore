using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;
using OnlineStore.WebAPI.Utilities;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<PizzaController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUserManager _userManager;
        private readonly TypedMemoryCache<int, UserDto> _memoryCache;

        public UserController(
            ILogger<PizzaController> logger,
            IUserRepository userRepository,
            IUserManager userManager,
            TypedMemoryCache<int, UserDto> memoryCache)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userManager = userManager;
            _memoryCache = memoryCache;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAll()
        {
            IEnumerable<User?> users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserDto(u));
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<ActionResult<UserDto?>> GetById(int id)
        {
            if (_memoryCache.TryGetValue(id, out UserDto? data) && data is not null)
            {
                return Ok(data);
            }

            User? user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                return BadRequest("User was not found.");
            }

            UserDto userDto = new(user);

            // Add to the cache
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(20)) // cache lifetime
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(50)); // max time
            _memoryCache.Set(id, userDto, cacheEntryOptions);

            return userDto;
        }
    }
}
