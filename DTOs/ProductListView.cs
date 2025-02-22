namespace gymvenience_backend.DTOs
{
    public class ProductListView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; private set; }
        public int Quantity { get; private set; }
        public double Price { get; private set; }
        public string CoverImageUrl { get; set; }
    }
}
