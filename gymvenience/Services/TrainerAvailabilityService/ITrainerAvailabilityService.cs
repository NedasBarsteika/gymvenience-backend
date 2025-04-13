﻿using gymvenience.Models;
using gymvenience_backend.DTOs;

namespace gymvenience.Services.TrainerAvailabilityService
{
    public interface ITrainerAvailabilityService
    {
        void AddSlot(string trainerId, DateTime date, TimeSpan start, TimeSpan end);
        IEnumerable<TrainerAvailability> GetAllSlotsForDate(string trainerId, DateTime date);
        IEnumerable<TrainerAvailability> GetAvailableSlotsForDate(string trainerId, DateTime date);
        public TrainerAvailability UpdateSlot(string slotId, UpdateTrainerSlotDto dto);
        public bool DeleteSlot(string slotId);
    }
}
