using gymvenience_backend.Repositories;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.OrderRepo;

namespace gymvenience_backend.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
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

            if (quantity > product.Quantity)
                throw new Exception($"This product has {product.Quantity} items in stock. Please enter a valid quantity");

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

        public async Task<Order> CheckoutAsync(string userId)
        {
            // Retrieve cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new Exception("Cart is empty");
            }

            // Retrieve all products in cart
            var productIds = cart.CartItems.Select(ci => ci.ProductId).ToList();
            var products = await _productRepository.GetByIdsAsync(productIds);

            // Create order from cart items
            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.CartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product {cartItem.ProductId} not found");
                }

                // Check if ordered quantity exceeds available stock
                if (cartItem.Quantity > product.Quantity)
                {
                    throw new Exception($"Not enough stock for {product.Name}. Available: {product.Quantity}, Requested: {cartItem.Quantity}");
                }

                // Deduct stock quantity
                product.Quantity -= cartItem.Quantity;
                await _productRepository.UpdateAsync(product);

                // Add to order
                orderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = product.Price
                });
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Items = orderItems
            };

            // Save order
            await _orderRepository.CreateOrderAsync(order);

            // Clear cart after successful order
            await _cartRepository.DeleteCartAsync(cart.Id);

            return order;
        }

    }
}