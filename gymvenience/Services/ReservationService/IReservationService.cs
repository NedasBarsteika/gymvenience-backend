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
    }
}
