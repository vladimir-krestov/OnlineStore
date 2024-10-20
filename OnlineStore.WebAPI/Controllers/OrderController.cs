using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Core.Models.Dto;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineStore.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private static int _connectionCounter = 0;

        private static object _locker = new();
        private static DateTime startTime;
        private static DateTime endTime;

        private readonly SemaphoreSlim _semaphore;
        private readonly ILoggerManager _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IPizzaRepository _pizzaRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(
            ILoggerManager logger,
            IOrderRepository orderRepository,
            IPizzaRepository pizzaRepository,
            IHttpContextAccessor httpContextAccessor,
            SemaphoreSlim semaphore)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _pizzaRepository = pizzaRepository;
            _httpContextAccessor = httpContextAccessor;
            _semaphore = semaphore;
        }

        //[Authorize(Roles = "StoreManager,Admin")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersFromPage([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            try
            {
                _logger.LogInfo($"Trying to get an order list filtered by pages, where {nameof(pageNumber)}:{pageNumber}, {nameof(pageSize)}:{pageSize}");

                List<Order> orders = await _orderRepository.GetOrdersFromPageAsync(pageNumber, pageSize);
                List<OrderDto> orderDtos = new List<OrderDto>();

                foreach (var order in orders)
                {
                    List<OrderItemDto> orderItemDtos = new();

                    foreach (var item in order.OrderItems)
                    {
                        OrderItemDto orderItemDto = new()
                        {
                            DoughType = item.DoughType,
                            OrderNumber = item.OrderNumber,
                            PizzaCount = item.PizzaCount,
                            PizzaSize = item.PizzaSize,
                            Pizza = new PizzaDto()
                            {
                                Category = item.Pizza.Category,
                                Description = item.Pizza.Description,
                                ImageUrl = item.Pizza.ImageUrl,
                                Price = item.Pizza.Price,
                                Title = item.Pizza.Title
                            }
                        };

                        orderItemDtos.Add(orderItemDto);
                    }

                    OrderDto orderDto = new()
                    {
                        CreationDate = order.CreationDate,
                        Number = order.Number,
                        State = order.State,
                        Total = order.Total,
                        OrderItems = orderItemDtos
                    };

                    orderDtos.Add(orderDto);
                }

                _logger.LogError("Test error");

                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
        }

        // TMP
        [HttpGet]
        [Route("GetOrderItemsByPizzaSize")]
        public async Task<ActionResult<int>> GetOrderItemsByPizzaSizeAsync([FromQuery] PizzaSize size)
        {
            try
            {
                List<OrderItem> orders = await _orderRepository.GetOrderItemsByPizzaSizeAsync(size);

                return Ok(orders.Count);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
        }

        // TMP
        [HttpGet]
        [Route("GetOrderItemsByAdditionalInfo")]
        public async Task<ActionResult<int>> GetOrderItemsByAdditionalInfoAsync([FromQuery] string info, [FromQuery] int size)
        {
            try
            {
                List<OrderItem> orders = await _orderRepository.GetOrderItemsByAdditionalInfoAsync(info, size);

                return Ok(orders.Count);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
        }

        //[Authorize(Roles = "StoreManager, Admin")]
        [HttpGet]
        [Route("GetOrderPageCount")]
        public async Task<ActionResult<int>> GetOrderPageCountAsync([FromQuery] int pageSize)
        {
            try
            {
                if (pageSize <= 0 || pageSize > 100)
                {
                    return BadRequest(new { message = "Page size is too large, use 1-100 values." });
                }

                int ordersCount = await _orderRepository.GetOrdersCountAsync();

                return Ok((ordersCount + pageSize - 1) / pageSize); // Like Math.Ceiling
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
        }

        [Authorize(Roles = "StoreManager, Admin")]
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

        [Authorize(Roles = "Customer")]
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

        //[Authorize(Roles = "Customer")]
        [HttpPost()]
        [Route("AddCustomerOrderItem")]
        public async Task<ActionResult<string>> AddCustomerOrderItemAsync([FromBody] OrderItemDto orderItemDto)
        {
            lock (_locker)
            {
                Debug.WriteLine(++_connectionCounter);

                if (_connectionCounter % 100 == 1)
                {
                    startTime = DateTime.UtcNow;
                    Debug.WriteLine($"Start time: {startTime}");
                }

                if (_connectionCounter % 100 == 0)
                {
                    endTime = DateTime.UtcNow;
                    Debug.WriteLine($"End time: {endTime}");
                    double seconds = (endTime - startTime).TotalSeconds;
                    Debug.WriteLine(seconds);
                }
            }

            await _semaphore.WaitAsync(); // Limits 30 connections at the same time, comes from DI

            try
            {
                // todo - return back
                int userId = 2;
                //string? userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue("UserId");
                //if (!int.TryParse(userIdClaim, out int userId))
                //{
                //    return BadRequest(new { message = "Can't get user id." });
                //}

                Pizza? pizza = await _pizzaRepository.GetPizzaByTitleAsync(orderItemDto.Pizza.Title);
                if (pizza is null)
                {
                    BadRequest(new { message = $"Pizza with title: {orderItemDto.Pizza.Title} hasn't been found." });
                }

                Order? relatedOrder;
                OrderItem orderItem = new(orderItemDto) { Pizza = pizza };
                double orderItemTotal = orderItem.Pizza.Price * orderItem.PizzaCount;
                if (string.IsNullOrEmpty(orderItemDto.OrderNumber))
                {
                    // Create new Order entry
                    relatedOrder = new Order()
                    {
                        Number = await _orderRepository.GenerateOrderNumber(),
                        OrderItems = new List<OrderItem>() { orderItem },
                        CreationDate = DateTime.UtcNow,
                        State = OrderState.Created,
                        UserId = userId,
                        Total = orderItemTotal
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
                    relatedOrder.Total += orderItemTotal;
                    relatedOrder.OrderItems.Add(orderItem);
                    await _orderRepository.AddOrderItemAsync(relatedOrder, orderItem);
                }

                return Ok(relatedOrder.Number);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error when getting orders.", error = ex.Message });
            }
            finally
            {
                _semaphore.Release();
            }
        }

        [Authorize(Roles = "StoreManager, Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Order?>> GetById(int id)
        {
            return Ok(await _orderRepository.GetByIdAsync(id));
        }
    }
}
