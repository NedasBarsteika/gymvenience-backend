using gymvenience_backend.Common;
using gymvenience_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace gymvenience_backend.Repositories.PurchaseRepo
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly ApplicationDbContext _context;
        public PurchaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> AddNewPurchaseAsync(Purchase purchase)
        {
            bool isAlreadyPurchased = await IsAlreadyPurchasedAsync(purchase);

            if (isAlreadyPurchased)
            {
                return Result.Failure("Product is already purchased");
            }

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<List<Purchase>> GetUserPurchasesAsync(string userId)
        {
            var userPurchases = await _context.Purchases
                .Include(r => r.PurchasedProduct)
                .Where(r => r.OwnerId == userId)
                .ToListAsync();

            return userPurchases;
        }

        public async Task<bool> IsAlreadyPurchasedAsync(Purchase purchase)
        {
            var userPurchases = await GetUserPurchasesAsync(purchase.OwnerId);

            foreach (var res in userPurchases)
            {
                if (res.PurchasedProduct.Id == purchase.PurchasedProduct.Id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
