using gymvenience.Models;
using gymvenience_backend;
using gymvenience_backend.DTOs;

namespace gymvenience.Repositories.TrainerAvailabilityRepo
{
    public class TrainerAvailabilityRepository : ITrainerAvailabilityRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainerAvailabilityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddAvailability(TrainerAvailability slot)
        {
            _context.TrainerAvailabilities.Add(slot);
            _context.SaveChanges();
        }
        public IEnumerable<TrainerAvailability> GetAllTrainerSlots(string trainerId, DateTime date)
        {
            return _context.TrainerAvailabilities
            .Where(a => a.TrainerId == trainerId && a.Date.Date == date.Date)
            .OrderBy(a => a.StartTime)
            .ToList();
        }

        public void DeleteAvailability(string id)
        {
            var slot = _context.TrainerAvailabilities.FirstOrDefault(a => a.Id == id);
            if (slot != null)
            {
                _context.TrainerAvailabilities.Remove(slot);
                _context.SaveChanges();
            }
        }
        public IEnumerable<TrainerAvailability> GetAvailableTrainerSlots(string trainerId, DateTime date)
        {
            return _context.TrainerAvailabilities
                .Where(slot =>
                    slot.TrainerId == trainerId &&
                    slot.Date.Date == date.Date &&
                    slot.Reserved == false
                )
                .OrderBy(slot => slot.StartTime)
                .ToList();
        }
        public TrainerAvailability? GetSlotById(string id)
        {
            return _context.TrainerAvailabilities.FirstOrDefault(s => s.Id == id);
        }

        public void DeleteSlot(TrainerAvailability slot)
        {
            _context.TrainerAvailabilities.Remove(slot);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void UpdateSlot(TrainerAvailability slot)
        {
            _context.TrainerAvailabilities.Update(slot);
        }
    }

}
