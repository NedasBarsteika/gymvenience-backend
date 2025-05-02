using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        public Task<List<Product>> GetAllAsync();
        public Task<Product?> GetByIdAsync(string id);
        Task<IEnumerable<Product>> GetByIdsAsync(List<string> productIds);
        Task UpdateAsync(Product product);
        Task<Product> AddAsync(Product product);
        Task<bool> DeleteAsync(string id);
        public Task<List<Product>> GetFilteredProductsAsync(string title, string category);
        void GenerateMockProducts();
    }
}
