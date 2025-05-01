using gymvenience_backend.Models;
using gymvenience_backend.DTOs;

namespace gymvenience_backend.Services.ProductService
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(string productId);
        public Task<List<Product>> SearchForProductsAsync(string query, string category);
        Task<Product> CreateProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(string id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(string id);
    }
}
