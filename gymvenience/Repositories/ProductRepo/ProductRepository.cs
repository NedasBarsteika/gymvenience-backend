using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace gymvenience_backend.Repositories.ProductRepo
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void GenerateMockProducts()
        {
            List<Product> newProducts = new List<Product>
                {
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Proteino milteliai",
                        Description = "Good product very nice",
                        Price = 24.99,
                        CoverImageUrl = "/Images/Products/protein_powder.jpg",
                        Category = "Baltymų milteliai",
                        Quantity = 50
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "20kg hanteliai",
                        Description = "Good product very nice",
                        Price = 49.99,
                        CoverImageUrl = "/Images/Products/20kg_dumbells.jpg",
                        Category = "Hanteliai",
                        Quantity = 50
                    }
            };

            _context.Products.AddRange(newProducts);
            _context.SaveChanges();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(b => b.Id == id);
            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        // Get multiple products by IDs
        public async Task<IEnumerable<Product>> GetByIdsAsync(List<string> productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();
        }

        // Update product details
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetFilteredProductsAsync(string name, string category)
        {
            List<Product> products = await _context.Products.ToListAsync();

            if (name != "")
            {
                products = products.Where(b => b.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (category.ToLower() != "any")
            {
                products = products.Where(b => b.Category.Trim().ToLower() == category.Trim().ToLower()).ToList();
            }

            return products;
        }
    }
}
