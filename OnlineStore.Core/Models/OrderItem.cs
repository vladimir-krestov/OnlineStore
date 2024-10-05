namespace OnlineStore.Core.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int PizzaId { get; set; }

        public Pizza Pizza { get; set; }

        public PizzaSize PizzaSize { get; set; }

        public DoughType DoughType { get; set; }

        public int PizzaCount { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }
    }
}
