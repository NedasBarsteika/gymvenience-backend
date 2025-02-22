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

        //public Product(string id, string name, string description, string category, int quantity, double price, string coverImageUrl)
        //{
        //    Id = id;
        //    Name = name;
        //    Description = description;
        //    Category = category;
        //    Quantity = quantity;
        //    Price = price;
        //    CoverImageUrl = coverImageUrl;
        //}

        //public Product() { }
    }
}
