namespace gymvenience_backend.DTOs
{
    public class OrderItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}