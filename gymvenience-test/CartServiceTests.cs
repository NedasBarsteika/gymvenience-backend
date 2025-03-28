using Xunit;
using FluentAssertions;
using Moq;
using gymvenience_backend.Services;
using gymvenience_backend.Repositories;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.OrderRepo;

namespace gymvenienceTest
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
    }
}