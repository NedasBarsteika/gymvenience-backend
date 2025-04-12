using gymvenience.Models;
using gymvenience_backend;
using Microsoft.EntityFrameworkCore;

namespace gymvenience.Repositories.GymRepo
{
    public class GymRepository : IGymRepository
    {
        private readonly ApplicationDbContext _context;

        public GymRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void GenerateMockGyms()
        {
            List<Gym> newGyms = new List<Gym>
                {
                    new Gym
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Dream Gym",
                        City = "Kaunas",
                        Address = "Geriausia g. 5-150",
                        CompanyName = "Gym-",
                        PhoneNumber = "(0-622) 04423",
                        Email = "dream.gym@gmail.com",
                        Description = "Best gym"
                    },
                    new Gym
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Dream Gym 2",
                        City = "Kaunas",
                        Address = "Geriausiškiausia g. 8-300",
                        CompanyName = "Gym-",
                        PhoneNumber = "(0-622) 04423",
                        Email = "dream.gym.2@gmail.com",
                        Description = "Best gym"
                    },
                    new Gym
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Dream Gym 3",
                        City = "Vilnius",
                        Address = "Geriausia g. 5-1500000",
                        CompanyName = "Gym-",
                        PhoneNumber = "(0-622) 04423",
                        Email = "dream.gym.3@gmail.com",
                        Description = "Best gym"
                    },
                    new Gym
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "",
                        City = "Klaipėda",
                        Address = "",
                        CompanyName = "",
                        PhoneNumber = "",
                        Email = "",
                        Description = ""
                    }
            };

            _context.Gyms.AddRange(newGyms);
            _context.SaveChanges();
        }

        public async Task<List<string>> GetAllCitiesAsync()
        {
            return await _context.Gyms.Select(g => g.City).Distinct().ToListAsync();
        }

        public async Task<List<string>> GetAllAddressesAsync()
        {
            return await _context.Gyms
                .Where(g => !string.IsNullOrEmpty(g.Address))
                .Select(g => g.Address)
                .Distinct()
                .ToListAsync();
        }
    }

}
