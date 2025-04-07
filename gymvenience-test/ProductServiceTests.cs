using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using gymvenience_backend;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Services.ProductService;

namespace gymvenience_test
{
    public class ProductServiceTests : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private IProductRepository _productRepository;
        private ProductService _productService;

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _productRepository = new ProductRepository(_context);
            _productService = new ProductService(_productRepository);

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
            var products = new List<Product>
            {
                new Product { Id = "1", Name = "Product A", Category = "Category 1", CoverImageUrl = "./Images/products/product_a.jpg", Description = "Best product", Price = 19.99, Quantity = 10 },
                new Product { Id = "2", Name = "Product B", Category = "Category 1", CoverImageUrl = "./Images/products/product_b.jpg", Description = "Best product", Price = 19.99, Quantity = 10 },
                new Product { Id = "3", Name = "Other Product", Category = "Category 2", CoverImageUrl = "./Images/products/other_product.jpg", Description = "Best product", Price = 19.99, Quantity = 10 }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task SearchForProductsAsync_Should_Return_Filtered_Products()
        {
            var result = await _productService.SearchForProductsAsync("Product", "Category 1");

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(p => p.Id).Should().Contain(new[] { "1", "2" });
        }

        [Fact]
        public async Task SearchForProductsAsync_Should_Return_Empty_List_When_No_Match()
        {
            var result = await _productService.SearchForProductsAsync("Nonexistent", "Category 1");

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Return_Product_When_Found()
        {
            var result = await _productService.GetProductByIdAsync("1");

            result.Should().NotBeNull();
            result!.Id.Should().Be("1");
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Return_Null_When_Not_Found()
        {
            var result = await _productService.GetProductByIdAsync("99");

            result.Should().BeNull();
        }
    }
}
