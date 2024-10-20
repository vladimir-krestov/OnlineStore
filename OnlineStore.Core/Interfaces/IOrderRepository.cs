using OnlineStore.Core.Models;
namespace OnlineStore.Core.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrdersFromPageAsync(int pageNumber, int pageSize);

        Task<int> GetOrdersCountAsync();

        Task<Order?> GetOrderByNumberAsync(string orderNumber);

        Task<List<Order>> GetOrdersByUserIdAsync(int userId);

        Task<bool> AddOrderAsync(Order order);

        Task<bool> AddOrderItemAsync(Order order, OrderItem orderItem);

        Task<string> GenerateOrderNumber();

        Task<List<OrderItem>> GetOrderItemsByPizzaSizeAsync(PizzaSize size);

        Task<List<OrderItem>> GetOrderItemsByAdditionalInfoAsync(string info, int size);
    }
}
