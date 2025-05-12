using gymvenience.Models;
using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.ReservationRepo
{
    public interface IReservationRepository
    {
        public IEnumerable<Reservation> GetUserReservations(string userId);
        public TrainerAvailability? GetAvailableTimeSlot(string slotId);
        public void MarkTimeSlotReserved(TrainerAvailability slot);
        public Gym? GetGymById(string gymId);
        public void AddReservation(Reservation reservation);
        public Reservation? GetReservationById(string reservationId);
        public void RemoveReservation(Reservation reservation);
        public void SaveChanges();
        public TrainerAvailability? GetTimeSlotByTrainerAndTime(string trainerId, DateTime date, TimeSpan startTime);
        Task SaveChangesAsync();
        IEnumerable<Reservation> GetReservationsByTrainer(string trainerId);
        IEnumerable<Reservation> GetAllReservations();
        User? GetUserById(string userId);
        IEnumerable<Reservation> GetCompletedReservationsByTrainer(string trainerId);
        IEnumerable<Reservation> GetPendingReservationsByTrainer(string trainerId);

    }
}