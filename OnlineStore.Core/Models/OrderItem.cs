using OnlineStore.Core.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OnlineStore.Core.Models
{
    public class OrderItem : DbModel
    {
        public OrderItem()
        {

        }

        public OrderItem(OrderItemDto orderItemDto)
        {
            PizzaSize = orderItemDto.PizzaSize;
            DoughType = orderItemDto.DoughType;
            PizzaCount = orderItemDto.PizzaCount;
            OrderNumber = orderItemDto.OrderNumber;
        }

        public int PizzaId { get; set; }

        public Pizza Pizza { get; set; }

        public PizzaSize PizzaSize { get; set; }

        public DoughType DoughType { get; set; }

        public int PizzaCount { get; set; }

        public string OrderNumber { get; set; }

        public Order Order { get; set; }

        [AllowNull]
        [MaxLength(100)]
        public string AdditionalInfo { get; set; }
    }
}
