using gymvenience.DTOs;
using gymvenience.Models;
using gymvenience_backend.DTOs;

namespace gymvenience.Services.ReservationService
{
    public interface IReservationService
    {
        public IEnumerable<Reservation> GetUserReservations(string userId);
        public bool CancelReservation(string reservationId);
        public ReservationResult CreateReservation(ReservationDto dto);
        Task<bool> MarkDoneAsync(string reservationId);
        IEnumerable<Reservation> GetReservationsForTrainer(string trainerId);
        IEnumerable<Reservation> GetAllReservations();
        Task<bool> ExistsForSessionAsync(string sessionId);
        Task CreateReservationAsync(Reservation reservation);
        Task<Reservation?> GetBySessionAsync(string sessionId);
    }
}
