using gymvenience_backend.Common;

namespace gymvenience_backend.Models
{
    public class Cart
    {
        public int Id { get; set; }
        
        // Foreign key to User
        public int UserId { get; set; }
        public User User { get; set; }
        
        // One cart can have many items
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
    
    public class CartItem
    {
        public int Id { get; set; }
        
        // Foreign key to Cart
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        
        // Foreign key to Product
        public string ProductId { get; set; }
        public Product Product { get; set; }
        
        public int Quantity { get; set; }
    }
}