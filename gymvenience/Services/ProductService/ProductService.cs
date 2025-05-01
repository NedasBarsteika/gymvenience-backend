using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.DTOs;

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

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }
        public async Task<Product> CreateProductAsync(CreateProductDto dto)
        {
            var prod = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Quantity = dto.Quantity,
                Price = dto.Price,
                CoverImageUrl = dto.CoverImageUrl
            };
            return await _productRepository.AddAsync(prod);
        }

        public async Task<bool> UpdateProductAsync(string id, UpdateProductDto dto)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return false;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Category = dto.Category;
            existing.Quantity = dto.Quantity;
            existing.Price = dto.Price;
            existing.CoverImageUrl = dto.CoverImageUrl;
            await _productRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteProductAsync(string id)
            => await _productRepository.DeleteAsync(id);

    }
}
