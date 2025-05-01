using gymvenience.Models;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore;

namespace gymvenience_backend.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            var user = await _context.Users
                             .Include(u => u.PurchasedProducts)
                             .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        // Profile edits (bio, trainer flag, etc.)
        public async Task<User?> UpdateProfileAsync(string userId, UserProfileDto updatedProfile)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            user.Bio = updatedProfile.Bio;
            user.IsTrainer = updatedProfile.IsTrainer;

            await _context.SaveChangesAsync();
            return user;
        }
        // Generic full-entity update (e.g. after demotion)
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Deleting user
        public async Task<bool> DeleteAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Getting users
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Promote user to trainer
        public async Task<bool> PromoteToTrainerAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsTrainer) return false;
            user.IsTrainer = true;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
