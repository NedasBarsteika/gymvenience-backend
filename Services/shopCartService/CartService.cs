using gymvenience_backend.Repositories;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;    

namespace gymvenience_backend.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository; 

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<Cart> GetOrCreateCartForUserAsync(int userId)
        {
            // Attempt to retrieve existing cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                await _cartRepository.CreateCartAsync(cart);
            }
            return cart;
        }

        public async Task<Cart> AddProductToCartAsync(int userId, string productId, int quantity)
        {
            // Validate product
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            // Get or create cart
            var cart = await GetOrCreateCartForUserAsync(userId);

            // Check if item already in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.Id
                };
                await _cartRepository.AddCartItemAsync(newItem);
            }

            // Return updated cart
            return await _cartRepository.GetCartByIdAsync(cart.Id);
        }

        public async Task<Cart> RemoveProductFromCartAsync(int userId, string productId, int quantity)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return null;

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem == null) return cart; // product not in cart, nothing to remove

        existingItem.Quantity -= quantity;
        if (existingItem.Quantity <= 0)
        {
            // remove item entirely
            await _cartRepository.RemoveCartItemAsync(existingItem.Id);
        }
        else
        {
            // update existing cart item using the repository method
            await _cartRepository.UpdateCartItemAsync(existingItem);
        }

            return await _cartRepository.GetCartByIdAsync(cart.Id);
        }

        public async Task<bool> ClearCartAsync(int cartId)
        {
            return await _cartRepository.DeleteCartAsync(cartId);
        }
    }
}