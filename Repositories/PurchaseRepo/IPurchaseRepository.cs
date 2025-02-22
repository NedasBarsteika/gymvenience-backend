using gymvenience_backend.Common;
using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.PurchaseRepo
{
    public interface IPurchaseRepository
    {
        public Task<List<Purchase>> GetUserPurchasesAsync(string userId);
        public Task<Result> AddNewPurchaseAsync(Purchase purchase);
        public Task<bool> IsAlreadyPurchasedAsync(Purchase purchase);
    }
}
