using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Services.ProductService;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gymvenience_test
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _productService = new ProductService(_productRepoMock.Object);
        }

        [Fact]
        public async Task SearchForProductsAsync_Should_Return_Filtered_Products()
        {
            // Arrange
            var expectedProducts = new List<Product>
        {
            new Product { Id = "1", Name = "Product A", Category = "Category 1" },
            new Product { Id = "2", Name = "Product B", Category = "Category 1" }
        };

            _productRepoMock.Setup(repo => repo.GetFilteredProductsAsync("Product", "Category 1"))
                            .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productService.SearchForProductsAsync("Product", "Category 1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0].Id);
            Assert.Equal("2", result[1].Id);
        }

        [Fact]
        public async Task SearchForProductsAsync_Should_Return_Empty_List_When_No_Match()
        {
            // Arrange
            _productRepoMock.Setup(repo => repo.GetFilteredProductsAsync("Nonexistent", "Category 1"))
                            .ReturnsAsync(new List<Product>());

            // Act
            var result = await _productService.SearchForProductsAsync("Nonexistent", "Category 1");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Return_Product_When_Found()
        {
            // Arrange
            var product = new Product { Id = "1", Name = "Product A" };

            _productRepoMock.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            _productRepoMock.Setup(repo => repo.GetByIdAsync("99")).ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetProductByIdAsync("99");

            // Assert
            Assert.Null(result);
        }
    }
}
