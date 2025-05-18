using gymvenience_backend.Common;
using gymvenience_backend.DTOs;
using gymvenience_backend.Models;

namespace gymvenience_backend.Services.UserService
{
    public interface IUserService
    {
        public Task<(Result, User?)> CreateUserAsync(string name, string surname, string email, string password);
        public Task<(Result, string)> GenerateJwtAsync(string email, string password);
        public Task<(Result, List<OrderDto>?)> GetAllOrdersAsync(string userId);
        bool IsPasswordCorrect(string password, string storedHashedPassword, string storedSalt);
        public Task<bool> DemoteTrainerAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> PromoteToTrainerAsync(string userId);
        Task<IEnumerable<User>> SearchTrainersByNameAsync(string searchText);
        Task<decimal> CalculateTrainerEarningsAsync(string trainerId);
        Task<bool> SetHourlyRateAsync(string trainerId, decimal newRate);
    }
}
