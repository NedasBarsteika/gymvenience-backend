using gymvenience.Models;
using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.ReservationRepo
{
    public interface IReservationRepository
    {
        public IEnumerable<Reservation> GetUserReservations(string userId);
    }
}