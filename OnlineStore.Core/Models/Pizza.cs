using OnlineStore.Core.Interfaces;

namespace OnlineStore.Core.Models
{
    public class Pizza : DbModel
    {
        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public PizzaCategory Category { get; set; }
    }
}
