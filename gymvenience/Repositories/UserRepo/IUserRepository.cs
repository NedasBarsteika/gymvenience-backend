using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.UserRepo
{
    public interface IUserRepository
    {
        public Task<User?> GetByIdAsync(string id);
        public Task<User?> GetByEmailAsync(string email);
        public Task AddAsync(User user);

        // Profile edits (bio, trainer flag, etc.)
        Task<User?> UpdateProfileAsync(string userId, UserProfileDto updatedProfile);

        // Generic full-entity update (e.g. after demotion)
        Task UpdateAsync(User user);
        Task<bool> DeleteAsync(string userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> PromoteToTrainerAsync(string userId);
    }
}
