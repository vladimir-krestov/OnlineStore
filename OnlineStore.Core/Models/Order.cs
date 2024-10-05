namespace OnlineStore.Core.Models
{
    public class Order : DbModel
    {
        public double Total { get; set; }

        public DateTime CreationDate { get; set; }

        public IEnumerable<OrderItem> OrderItems { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public OrderState State { get; set; }
    }
}
