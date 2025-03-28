using gymvenience_backend.Common;

namespace gymvenience_backend.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
    
    public class CartItem
    {
        public int Id { get; set; }    
        public int CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}