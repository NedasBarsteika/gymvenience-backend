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

        public async Task<Cart> GetOrCreateCartForUserAsync(string userId)
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

        public async Task<Cart> AddProductToCartAsync(string userId, string productId, int quantity)
        {
            // Validate product
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            // Get or create cart
            var cart = await GetOrCreateCartForUserAsync(userId);

            // Check if item already in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (quantity <= 0 && existingItem != null)
            {
                await _cartRepository.RemoveCartItemAsync(existingItem.Id);
                return await _cartRepository.GetCartByIdAsync(cart.Id);
            }
            if (quantity <= 0)
            {
                return await _cartRepository.GetCartByIdAsync(cart.Id);
            }

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
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

        public async Task<Cart> RemoveProductFromCartAsync(string userId, string productId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return null;

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem == null) return cart; // product not in cart, nothing to remove

            existingItem.Quantity = 0;
            await _cartRepository.RemoveCartItemAsync(existingItem.Id);

            return await _cartRepository.GetCartByIdAsync(cart.Id);
        }

        public async Task<bool> ClearCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            return await _cartRepository.DeleteCartAsync(cart.Id);
        }
    }
}