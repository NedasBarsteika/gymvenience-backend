using Xunit;
using FluentAssertions;
using Moq;
using gymvenience_backend.Services;
using gymvenience_backend.Repositories;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.OrderRepo;

namespace gymvenience_test
{
    public class CartServiceTest
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly CartService _cartService;

        public CartServiceTest()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _cartService = new CartService(_cartRepositoryMock.Object, _productRepositoryMock.Object, _orderRepositoryMock.Object);
        }

        [Fact]
        public async Task AddProductToCart_Should_Add_Item_When_Stock_Is_Available()
        {
            var userId = "user123";
            var product = new Product { Id = "1", Name = "Protein Powder", Quantity = 10, Price = 19.99 };
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _cartRepositoryMock.Setup(repo => repo.AddCartItemAsync(It.IsAny<CartItem>()));

            var result = await _cartService.AddProductToCartAsync(userId, product.Id, 2);

            _cartRepositoryMock.Verify(repo => repo.AddCartItemAsync(It.IsAny<CartItem>()), Times.Once);
        }

        [Fact]
        public async Task AddProductToCart_Should_Throw_Exception_When_Product_Not_Found()
        {
            // Arrange
            var userId = "user123";
            var productId = "nonexistent-product";
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cartService.AddProductToCartAsync(userId, productId, 2));
            Assert.Equal("Product not found", exception.Message);
        }

        [Fact]
        public async Task AddProductToCart_Should_Throw_Exception_When_Quantity_Exceeds_Available_Stock()
        {
            // Arrange
            var userId = "user123";
            var product = new Product { Id = "1", Name = "Protein Powder", Quantity = 2, Price = 19.99 };
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(product.Id)).ReturnsAsync(product);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _cartService.AddProductToCartAsync(userId, product.Id, 5));
            Assert.Equal($"This product has {product.Quantity} items in stock. Please enter a valid quantity", exception.Message);
        }

        [Fact]
        public async Task RemoveProductFromCart_Should_Remove_Existing_Item()
        {
            var userId = "user123";
            var productId = "1";
            var cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem> { new CartItem { ProductId = productId, Quantity = 1, Id = 1 } }
            };

            _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
            _cartRepositoryMock.Setup(repo => repo.RemoveCartItemAsync(It.IsAny<int>()));
            _cartRepositoryMock.Setup(repo => repo.GetCartByIdAsync(It.IsAny<int>())).ReturnsAsync(new Cart { UserId = userId, CartItems = new List<CartItem>() });

            var result = await _cartService.RemoveProductFromCartAsync(userId, productId);

            result.Should().NotBeNull();
            result.CartItems.Should().BeEmpty();
        }

        [Fact]
        public async Task ClearCartByUserId_Should_Return_True_When_Cart_Exists()
        {
            string userId = "user123";
            var cart = new Cart { Id = 1, UserId = userId };
            _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
            _cartRepositoryMock.Setup(repo => repo.DeleteCartAsync(cart.Id)).ReturnsAsync(true);

            var result = await _cartService.ClearCartByUserIdAsync(userId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Checkout_Should_Succeed_When_Stock_Is_Available()
        {
            var userId = "user123";
            var cart = new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem> { new CartItem { ProductId = "1", Quantity = 2 } }
            };
            var product = new Product { Id = "1", Name = "Protein Powder", Quantity = 5, Price = 19.99 };

            _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
            _productRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<string>>())).ReturnsAsync(new List<Product> { product });
            _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _orderRepositoryMock.Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>()));

            var result = await _cartService.CheckoutAsync(userId);

            result.Should().NotBeNull();
            result.Items.Should().ContainSingle(item => item.ProductId == "1" && item.Quantity == 2);
        }

        [Fact]
        public async Task Checkout_Should_Throw_Exception_When_Cart_Is_Empty()
        {
            string userId = "user123";
            _cartRepositoryMock.Setup(repo => repo.GetCartByUserIdAsync(userId)).ReturnsAsync((Cart)null);

            Func<Task> act = async () => await _cartService.CheckoutAsync(userId);

            await act.Should().ThrowAsync<Exception>().WithMessage("Cart is empty");
        }
    }
}