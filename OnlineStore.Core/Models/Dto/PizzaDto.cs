namespace OnlineStore.Core.Models.Dto
{
    public class PizzaDto
    {
        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public PizzaCategory Category { get; set; }
    }
}
