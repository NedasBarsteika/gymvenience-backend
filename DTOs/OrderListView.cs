namespace gymvenience_backend.DTOs
{
    public class OrderListView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; private set; }
        public int Quantity { get; private set; }
        public double Price { get; private set; }
        public string Description { get; private set; }
        public string CoverImageUrl { get; set; }
    }
}
