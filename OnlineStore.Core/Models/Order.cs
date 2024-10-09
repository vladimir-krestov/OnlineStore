using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Core.Models
{
    public class Order : DbModel
    {
        [MaxLength(8)]
        public string Number { get; set; }

        public double Total { get; set; }

        public DateTime CreationDate { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public OrderState State { get; set; }
    }
}
