using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;
using System.Security.Claims;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<PizzaController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(ILogger<PizzaController> logger, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize("StoreManager, Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                return Ok(await _orderRepository.GetAllAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
        }

        [Authorize("StoreManager, Admin")]
        [HttpGet]
        [Route("GetOrdersByUserId/{id}")]
        public async Task<ActionResult<List<Order>>> GetOrdersByUserIdAsync(int userId)
        {
            try
            {
                return Ok(await _orderRepository.GetOrdersByUserIdAsync(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
        }

        [Authorize("Customer")]
        [HttpGet]
        [Route("GetCustomerOrders")]
        public async Task<ActionResult<List<Order>>> GetCustomerOrdersAsync()
        {
            try
            {
                string? userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Can't get user id." });
                }

                return Ok(await _orderRepository.GetOrdersByUserIdAsync(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
        }

        [Authorize("Customer")]
        [HttpPost()]
        [Route("AddCustomerOrderItem")]
        public async Task<ActionResult<bool>> AddCustomerOrderItemAsync([FromBody] OrderItemDto orderItemDto)
        {
            try
            {
                string? userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("UserId");
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new { message = "Can't get user id." });
                }

                Order? relatedOrder;
                OrderItem orderItem = new(orderItemDto);
                if (string.IsNullOrEmpty(orderItemDto.OrderNumber))
                {
                    // Create new Order entry
                    relatedOrder = new Order()
                    {
                        Number = await _orderRepository.GenerateOrderNumber(),
                        OrderItems = new List<OrderItem>() { orderItem },
                        CreationDate = DateTime.UtcNow,
                        State = OrderState.Created,
                        UserId = userId
                    };

                    await _orderRepository.AddOrderAsync(relatedOrder);
                }
                else
                {
                    relatedOrder = await _orderRepository.GetOrderByNumberAsync(orderItemDto.OrderNumber);
                    if (relatedOrder is null)
                    {
                        return BadRequest(new { message = $"Order can't be found with id = {orderItemDto.OrderNumber}." });
                    }

                    // Save order
                    relatedOrder.OrderItems.Add(orderItem);
                    await _orderRepository.AddOrderItemAsync(relatedOrder, orderItem);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }

            
        }

        [Authorize("StoreManager, Admin")]
        [HttpGet("{id}")]
        public async Task<Order?> GetById(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }
    }
}
