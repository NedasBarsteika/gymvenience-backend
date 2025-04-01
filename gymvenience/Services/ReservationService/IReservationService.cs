using gymvenience.Models;

namespace gymvenience.Services.ReservationService
{
    public interface IReservationService
    {
        public IEnumerable<Reservation> GetUserReservations(int userId);
    }
}
