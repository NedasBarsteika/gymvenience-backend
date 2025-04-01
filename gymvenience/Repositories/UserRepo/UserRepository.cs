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

        public async Task<User?> UpdateUserAsync(string userId, UserProfileDto updatedProfile)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            // Update fields
            user.Bio = updatedProfile.Bio;
            user.IsTrainer = updatedProfile.IsTrainer;
            // ... update other fields as needed

            await _context.SaveChangesAsync();
            return user;
        }
    }
}
