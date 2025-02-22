using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;

namespace gymvenience_backend.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> SearchForProductsAsync(string title, string category)
        {
            var products = await _productRepository.GetFilteredProductsAsync(title, category);
            return products;
        }
    }
}
