namespace gymvenience_backend.DTOs
{
public class CheckoutSessionRequest
{
    public decimal TotalPrice { get; set; }
    public string ShippingMethod { get; set; }
    public string? UserId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}
}