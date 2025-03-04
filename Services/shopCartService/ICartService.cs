using gymvenience_backend.Models;
namespace gymvenience_backend.Services
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartForUserAsync(int userId);
        Task<Cart> AddProductToCartAsync(int userId, string productId, int quantity);
        Task<Cart> RemoveProductFromCartAsync(int userId, string productId, int quantity);
        Task<bool> ClearCartAsync(int cartId);
    }
}