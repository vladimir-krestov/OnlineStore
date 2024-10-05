using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : ControllerBase
    {
        private readonly ILogger<PizzaController> _logger;
        private readonly IRepository<Pizza> _pizzaRepository;

        public PizzaController(ILogger<PizzaController> logger, IRepository<Pizza> pizzaRepository)
        {
            _logger = logger;
            _pizzaRepository = pizzaRepository;
        }

        [HttpGet(Name = "GetAllPizza")]
        public async Task<IEnumerable<Pizza>> GetAll()
        {
            return await _pizzaRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<Pizza?> GetById(int id)
        {
            return await _pizzaRepository.GetByIdAsync(id);
        }
    }
}
