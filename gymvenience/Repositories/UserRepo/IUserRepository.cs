using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.UserRepo
{
    public interface IUserRepository
    {
        public Task<User?> GetByIdAsync(string id);
        public Task<User?> GetByEmailAsync(string email);
        public Task AddAsync(User user);
        public Task<User?> UpdateUserAsync(string userId, UserProfileDto updatedProfile);
    }
}
