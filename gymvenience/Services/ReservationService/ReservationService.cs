using gymvenience.Models;
using gymvenience.Services.ReservationService;
using gymvenience_backend.Repositories.ReservationRepo;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public IEnumerable<Reservation> GetUserReservations(string userId)
    {
        return _reservationRepository.GetUserReservations(userId);
    }
}