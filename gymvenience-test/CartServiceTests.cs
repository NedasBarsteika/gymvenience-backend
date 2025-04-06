using Xunit;
using FluentAssertions;
using gymvenience_backend.Services;
using gymvenience_backend.Repositories;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.OrderRepo;
using Microsoft.EntityFrameworkCore;
using gymvenience_backend;

namespace gymvenience_test
{
    public class CartServiceTest : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private CartService _cartService;
        private ICartRepository _cartRepository;
        private IProductRepository _productRepository;
        private IOrderRepository _orderRepository;

        private string userId = "test-user";

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _cartRepository = new CartRepository(_context);
            _productRepository = new ProductRepository(_context);
            _orderRepository = new OrderRepository(_context);

            _cartService = new CartService(_cartRepository, _productRepository, _orderRepository);

            await SeedDataAsync();
        }

        public Task DisposeAsync()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            return Task.CompletedTask;
        }

        private async Task SeedDataAsync()
        {
            _context.Products.Add(new Product
            {
                Id = "prod-1",
                Name = "Whey Protein",
                Quantity = 10,
                Price = 25.0,
                Category = "Protein Powder",
                CoverImageUrl = "./Images/products/whey_protein.jpg",
                Description = "Best product"
            });

            _context.Products.Add(new Product
            {
                Id = "prod-2",
                Name = "BCAA",
                Quantity = 2,
                Price = 10.0,
                Category = "product",
                CoverImageUrl = "./Images/products/product.jpg",
                Description = "Best product"
            });

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task AddProductToCart_Should_Add_Valid_Item()
        {
            var result = await _cartService.AddProductToCartAsync(userId, "prod-1", 3);

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            cart.Should().NotBeNull();
            cart.CartItems.Should().ContainSingle(x => x.ProductId == "prod-1" && x.Quantity == 3);
        }

        [Fact]
        public async Task AddProductToCart_Should_Throw_If_Product_Not_Found()
        {
            var act = async () => await _cartService.AddProductToCartAsync(userId, "invalid-id", 2);

            await act.Should().ThrowAsync<Exception>().WithMessage("Product not found");
        }

        [Fact]
        public async Task AddProductToCart_Should_Throw_If_Quantity_Too_High()
        {
            var act = async () => await _cartService.AddProductToCartAsync(userId, "prod-2", 5);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("This product has 2 items in stock. Please enter a valid quantity");
        }

        [Fact]
        public async Task Checkout_Should_Create_Order_And_Update_Stock()
        {
            await _cartService.AddProductToCartAsync(userId, "prod-1", 2);

            var order = await _cartService.CheckoutAsync(userId);

            order.Should().NotBeNull();
            order.Items.Should().ContainSingle(x => x.ProductId == "prod-1" && x.Quantity == 2);

            var updatedProduct = await _productRepository.GetByIdAsync("prod-1");
            updatedProduct.Quantity.Should().Be(8);
        }

        [Fact]
        public async Task Checkout_Should_Throw_If_Cart_Is_Empty()
        {
            var act = async () => await _cartService.CheckoutAsync(userId);

            await act.Should().ThrowAsync<Exception>().WithMessage("Cart is empty");
        }

        [Fact]
        public async Task ClearCart_Should_Remove_Cart()
        {
            await _cartService.AddProductToCartAsync(userId, "prod-1", 1);
            var result = await _cartService.ClearCartByUserIdAsync(userId);

            result.Should().BeTrue();

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            cart.Should().BeNull();
        }
    }
}
