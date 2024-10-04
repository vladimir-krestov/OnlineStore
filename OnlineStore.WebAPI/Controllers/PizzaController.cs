using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Infrastructure.Data;
using OnlineStore.Infrastructure.Repositories;

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
