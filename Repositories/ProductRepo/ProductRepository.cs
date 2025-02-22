using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using Microsoft.EntityFrameworkCore;

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
                        Name = "Protein Powder",
                        Description = "Good product very nice",
                        Price = 24.99,
                        CoverImageUrl = "https://www.google.com/search?sca_esv=3473e858e21315cd&rlz=1C1GCEA_enLT1141LT1141&sxsrf=AHTn8zo2efG1ZNKRF_o-0AJG9pfIjqgVoA:1740225083696&q=protein+powder&udm=2&fbs=ABzOT_CWdhQLP1FcmU5B0fn3xuWpA-dk4wpBWOGsoR7DG5zJBnsX62dbVmWR6QCQ5QEtPRqInn_ud8BG5tWxPdEiOpZiSu2wfu34gGffy1i9A3JULfbcTBMGVZszoFuX3xY9s3daoHWPzXuozsuX3n8GpOZWGjaNsKcNnasQIn5Pz6E3QnYuIBc&sa=X&sqi=2&ved=2ahUKEwitosXPm9eLAxUh_gIHHbW3PBsQtKgLegQIFhAB&biw=1536&bih=695&dpr=1.25#vhid=3UZjvrV9Q4NJBM&vssid=mosaic",
                        Category = "ProteinPowder"
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "20kg dumbbells",
                        Description = "Good product very nice",
                        Price = 49.99,
                        CoverImageUrl = "https://www.google.com/search?q=20+kg+dumbbells&sca_esv=3473e858e21315cd&rlz=1C1GCEA_enLT1141LT1141&udm=2&biw=1536&bih=695&sxsrf=AHTn8zqbvm5Qe9qfghb6NoP7VCulVmv-ag%3A1740225385636&ei=abu5Z5WyJoKji-gP-teW-QQ&ved=0ahUKEwjVisLfnNeLAxWC0QIHHfqrJU8Q4dUDCBE&uact=5&oq=20+kg+dumbbells&gs_lp=EgNpbWciDzIwIGtnIGR1bWJiZWxsczIGEAAYBxgeMgYQABgHGB4yBhAAGAcYHjIGEAAYBxgeMgYQABgHGB4yBhAAGAcYHjIGEAAYBxgeMgYQABgHGB4yBhAAGAcYHjIGEAAYBxgeSKISUP4GWK4QcAB4AJABAJgBUaABiASqAQE3uAEDyAEA-AEBmAIGoALXA8ICChAAGIAEGEMYigXCAggQABgHGAoYHpgDAOIDBRIBMSBAiAYBkgcBNqAH-yQ&sclient=img#vhid=-C5-6djJzoGRjM&vssid=mosaic",
                        Category = "Dumbells"
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

        public async Task<List<Product>> GetFilteredProductsAsync(string name, string category)
        {
            List<Product> products = await _context.Products.ToListAsync();

            if (name != "")
            {
                products = products.Where(b => b.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            if (category.ToLower() != "any")
            {
                products = products.Where(b => b.Category.ToLower() == category).ToList();
            }

            return products;
        }
    }
}
