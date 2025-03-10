using gymvenience_backend.Models;
namespace gymvenience_backend.Services
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartForUserAsync(string userId);
        Task<Cart> AddProductToCartAsync(string userId, string productId, int quantity);
        Task<Cart> RemoveProductFromCartAsync(string userId, string productId, int quantity);
        Task<bool> ClearCartAsync(int cartId);
    }
}