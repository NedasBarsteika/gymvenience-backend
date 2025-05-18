namespace gymvenience_backend.DTOs
{
    public class OrderDto
    {
        public int    Id            { get; set; }
        public DateTime OrderDate   { get; set; }
        public bool   IsDelivered   { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}