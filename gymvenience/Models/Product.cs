namespace gymvenience_backend.Models
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public int Quantity { get; set; }

        public double Price {  get; set; }

        public string CoverImageUrl { get; set; }

    }
}
