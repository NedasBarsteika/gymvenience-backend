using gymvenience.Models;

namespace gymvenience.DTOs
{
    public class ReservationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Reservation? Reservation { get; set; }

        public ReservationResult(bool success, string message, Reservation? reservation = null)
        {
            Success = success;
            Message = message;
            Reservation = reservation;
        }
    }
}
