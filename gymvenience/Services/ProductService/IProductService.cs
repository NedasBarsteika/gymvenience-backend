using gymvenience_backend.Models;

namespace gymvenience_backend.Services.ProductService
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(string productId);
        public Task<List<Product>> SearchForProductsAsync(string query, string category);
    }
}
