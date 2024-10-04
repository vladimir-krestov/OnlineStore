namespace OnlineStore.Core.Models
{
    public class Pizza : Product
    {
        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        // Ref to categories
        public int CategoryId { get; set; }
        public Category Category{ get; set; }
    }
}
