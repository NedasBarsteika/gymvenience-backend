using gymvenience.Models;

public interface ITrainerAvailabilityRepository
{
    void AddAvailability(TrainerAvailability slot);
    IEnumerable<TrainerAvailability> GetAllTrainerSlots(string trainerId, DateTime date);
    void DeleteAvailability(string id);
    IEnumerable<TrainerAvailability> GetAvailableTrainerSlots(string trainerId, DateTime date);
    public void UpdateSlot(TrainerAvailability slot);
    public void SaveChanges();
    public void DeleteSlot(TrainerAvailability slot);
    public TrainerAvailability? GetSlotById(string id);
}
