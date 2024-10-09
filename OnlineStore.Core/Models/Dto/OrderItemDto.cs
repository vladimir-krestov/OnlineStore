namespace OnlineStore.Core.Models.Dto
{
    public class OrderItemDto
    {
        public PizzaDto Pizza { get; set; }

        public PizzaSize PizzaSize { get; set; }

        public DoughType DoughType { get; set; }

        public int PizzaCount { get; set; }

        public string OrderNumber { get; set; }
    }
}
