using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Interfaces;
using OnlineStore.Core.Models;
using OnlineStore.Infrastructure.Data;
using System.Linq.Expressions;

namespace OnlineStore.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationContext _context;

        public OrderRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Order>> GetOrdersFromPageAsync(int pageNumber, int pageSize)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Pizza)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<int> GetOrdersCountAsync()
        {
            return _context.Orders.CountAsync();
        }

        public Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        }

        public async Task<bool> AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);

            int saveEntries = await _context.SaveChangesAsync();

            // Returns if operation was successful.
            return saveEntries > 0;
        }

        public async Task<bool> AddOrderItemAsync(Order order, OrderItem orderItem)
        {
            order.OrderItems.Add(orderItem);

            int saveEntries = await _context.SaveChangesAsync();

            // Returns if operation was successful.
            return saveEntries > 0;
        }

        public async Task<bool> SaveOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);

            int saveEntries = await _context.SaveChangesAsync();

            // Returns if operation was successful.
            return saveEntries > 0;
        }

        public async Task<string> GenerateOrderNumber()
        {
            const int NumberLength = 8;
            const int Attempts = 10;

            string? orderNumber = null;

            for (int i = 0; i < Attempts; i++)
            {
                string generatedNumber = GenerateNumber(NumberLength);

                if (!(await _context.Orders.AnyAsync(o => o.Number == generatedNumber)))
                {
                    orderNumber = generatedNumber;
                    break;
                }
            }

            if (orderNumber is null)
            {
                throw new OperationCanceledException("Too many attempts to generate an order unique number.");
            }

            return orderNumber;

            static string GenerateNumber(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                                  .Select(s => s[new Random().Next(s.Length)]).ToArray());
            }
        }

        public Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            return _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Number == orderNumber);
        }

        public Task<List<OrderItem>> GetOrderItemsByPizzaSizeAsync(PizzaSize size)
        {
            return _context.OrderItems.Where(oi => oi.PizzaSize == size).ToListAsync();
        }

        public Task<List<OrderItem>> GetOrderItemsByAdditionalInfoAsync(string info, int size)
        {
            Expression<Func<OrderItem, bool>> filter = size > 0
                ? oi => oi.AdditionalInfo == info && oi.PizzaSize == (PizzaSize)size
                : oi => oi.AdditionalInfo == info;

            return _context.OrderItems.Where(filter).ToListAsync();
        }
    }
}
