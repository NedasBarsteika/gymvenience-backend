using gymvenience_backend.Models;

namespace gymvenience_backend.Services.ProductService
{
    public interface IProductService
    {
        public Task<List<Product>> SearchForProductsAsync(string query, string category);
    }
}
