using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaController : ControllerBase
    {
        private readonly ILogger<PizzaController> _logger;
        private readonly IPizzaRepository _pizzaRepository;

        public PizzaController(ILogger<PizzaController> logger, IPizzaRepository pizzaRepository)
        {
            _logger = logger;
            _pizzaRepository = pizzaRepository;
        }

        [HttpGet]
        [Route("GetAllPizza")]
        public async Task<IEnumerable<Pizza>> GetAll()
        {
            return await _pizzaRepository.GetAllAsync();
        }

        [HttpGet]
        [Route("GetPizzaById/{id}")]
        public async Task<Pizza?> GetById(int id)
        {
            return await _pizzaRepository.GetByIdAsync(id);
        }

        [HttpPost]
        [Route("CreateNewPizza")]
        public async Task<bool> CreateNewPizza([FromBody]PizzaDto pizzaDto)
        {
            return await _pizzaRepository.CreateNewPizza(pizzaDto);
        }
    }
}
