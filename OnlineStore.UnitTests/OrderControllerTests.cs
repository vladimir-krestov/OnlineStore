using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Infrastructure.Repositories;
using OnlineStore.WebAPI.Controllers;
using System.Threading;
using NUnit.Framework.Legacy;

namespace OnlineStore.UnitTests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private OrderController _controller;
        private Mock<ILoggerManager> _loggerMock;
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IPizzaRepository> _pizzaRepositoryMockMock;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private SemaphoreSlim _semaphore;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILoggerManager>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderRepositoryMock.Setup(o => o.GetOrdersCountAsync()).ReturnsAsync(1000);
            _orderRepositoryMock.Setup(o => o.GetByIdAsync(1))
                .ReturnsAsync(new Order() { Id = 1, Number = "ABCD1234" });
            _orderRepositoryMock.Setup(o => o.GetByIdAsync(2))
                .ReturnsAsync(new Order() { Id = 2, Number = "1234ABCD" });
            _pizzaRepositoryMockMock = new Mock<IPizzaRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _semaphore = new SemaphoreSlim(90);
            _controller = new OrderController(
                logger: _loggerMock.Object,
                orderRepository: _orderRepositoryMock.Object,
                pizzaRepository: _pizzaRepositoryMockMock.Object,
                httpContextAccessor: _httpContextAccessor.Object,
                semaphore: _semaphore);
        }

        [Test]
        public async Task GetOrderPageCountAsync_PazeSizeInRage_ReturnsOkResult()
        {
            // Arrange
            // Act
            ActionResult<int> pageCount = await _controller.GetOrderPageCountAsync(pageSize: 20);

            // Assert 
            Assert.That((pageCount.Result as ObjectResult)?.StatusCode, Is.EqualTo(200));
            Assert.That((pageCount.Result as ObjectResult)?.Value, Is.EqualTo(1000/20));
        }

        [Test]
        [TestCase(1, "ABCD1234")]
        [TestCase(2, "1234ABCD")]
        public async Task GetById_OrderExists_ReturnsOkResult(int orderId, string expectedOrderNumber)
        {
            // Arrange
            // Act
            ActionResult<Order?> pageCount = await _controller.GetById(id: orderId);
            ObjectResult? actionResult = pageCount.Result as ObjectResult;
            Order? orderResult = actionResult?.Value as Order;

            // Assert 
            ClassicAssert.IsNotNull(actionResult);
            ClassicAssert.IsNotNull(orderResult);
            Assert.That(actionResult!.StatusCode, Is.EqualTo(200));
            Assert.That(orderResult!.Id, Is.EqualTo(orderId));
            Assert.That(orderResult!.Number, Is.EqualTo(expectedOrderNumber));
        }
    }
}
