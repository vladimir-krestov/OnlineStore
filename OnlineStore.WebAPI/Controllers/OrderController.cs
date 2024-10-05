using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<PizzaController> _logger;
        private readonly IRepository<Order> _orderRepository;

        public OrderController(ILogger<PizzaController> logger, IRepository<Order> orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [HttpGet(Name = "GetAllOrders")]
        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<Order?> GetById(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }
    }
}
