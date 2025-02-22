using gymvenience_backend.Common;
using gymvenience_backend.Models;

namespace gymvenience_backend.Services.UserService
{
    public interface IUserService
    {
        public Task<(Result, User?)> CreateUserAsync(string name, string surname, string email, string password);
        public Task<(Result, string)> GenerateJwtAsync(string email, string password);
        public Task<Result> AddNewPurchaseAsync(string userId, string productId, ProductType category);
        public Task<(Result, List<Purchase>?)> GetAllPurchasesAsync(string userId);
        bool IsPasswordCorrect(string password, string storedHashedPassword, string storedSalt);
    }
}
