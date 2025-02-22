using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetAllAsync();
        public Task<Product?> GetByIdAsync(string id);
        public Task<List<Product>> GetFilteredProductsAsync(string title, string category);
        void GenerateMockProducts();
    }
}
