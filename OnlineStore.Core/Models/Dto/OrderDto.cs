namespace OnlineStore.Core.Models.Dto
{
    public class OrderDto
    {
        public string Number { get; set; }

        public double Total { get; set; }

        public DateTime CreationDate { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }

        public OrderState State { get; set; }
    }
}
