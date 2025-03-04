using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task<Cart> GetCartByIdAsync(int cartId);
        Task<Cart> CreateCartAsync(Cart cart);
        Task<Cart> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(int cartId);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);

        // Possibly methods for adding/removing CartItems directly
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<bool> RemoveCartItemAsync(int cartItemId);
    }
}