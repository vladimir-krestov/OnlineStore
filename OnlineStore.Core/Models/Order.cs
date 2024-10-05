namespace OnlineStore.Core.Models
{
    public class Order
    {
        public int Id { get; set; }

        public double Total { get; set; }

        public DateTime CreationDate { get; set; }

        public IEnumerable<ConfiguredPizzaOrderItem> ConfiguredPizzaOrderItems { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public OrderState State { get; set; }
    }
}
