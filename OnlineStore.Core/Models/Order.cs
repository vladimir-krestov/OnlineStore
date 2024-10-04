using System;
namespace OnlineStore.Core.Models
{
    public class Order
    {
        public int Id { get; set; }

        public double Total { get; set; }

        public DateTime Date { get; set; }

        public IEnumerable<PizzaOrder> PizzaOrders { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
