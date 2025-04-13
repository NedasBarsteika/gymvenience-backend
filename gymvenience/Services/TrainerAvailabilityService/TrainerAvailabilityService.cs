using gymvenience.Models;
using gymvenience.Services.TrainerAvailabilityService;
using gymvenience_backend.DTOs;
using Microsoft.EntityFrameworkCore;

public class TrainerAvailabilityService : ITrainerAvailabilityService
{
    private readonly ITrainerAvailabilityRepository _trainerAvailabilityRepository;

    public TrainerAvailabilityService(ITrainerAvailabilityRepository repository)
    {
        _trainerAvailabilityRepository = repository;
    }

    public void AddSlot(string trainerId, DateTime date, TimeSpan start, TimeSpan end)
    {
        // Check for overlaps before adding
        var slot = new TrainerAvailability
        {
            TrainerId = trainerId,
            Date = date,
            StartTime = start,
            EndTime = end
        };
        _trainerAvailabilityRepository.AddAvailability(slot);
    }

    public IEnumerable<TrainerAvailability> GetAllSlotsForDate(string trainerId, DateTime date)
    {
        return _trainerAvailabilityRepository.GetAllTrainerSlots(trainerId, date);
    }
    public IEnumerable<TrainerAvailability> GetAvailableSlotsForDate(string trainerId, DateTime date)
    {
        return _trainerAvailabilityRepository.GetAvailableTrainerSlots(trainerId, date);
    }
    public bool DeleteSlot(string slotId)
    {
        var slot = _trainerAvailabilityRepository.GetSlotById(slotId);
        if (slot == null || slot.Reserved)
        {
            return false; // Slot doesn't exist or already reserved
        }

        _trainerAvailabilityRepository.DeleteSlot(slot);
        _trainerAvailabilityRepository.SaveChanges();
        return true;
    }

    public TrainerAvailability UpdateSlot(string slotId, UpdateTrainerSlotDto dto)
    {
        var slot = _trainerAvailabilityRepository.GetSlotById(slotId);
        if (slot == null || slot.Reserved)
        {
            return null; // Can't update reserved slot
        }

        slot.Date = dto.Date;
        slot.StartTime = dto.StartTime;
        slot.EndTime = dto.EndTime;

        _trainerAvailabilityRepository.UpdateSlot(slot);
        _trainerAvailabilityRepository.SaveChanges();

        return slot;
    }

}
