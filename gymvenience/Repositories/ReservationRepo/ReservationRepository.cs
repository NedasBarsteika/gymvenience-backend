using gymvenience.Models;
using gymvenience_backend;
using gymvenience_backend.Repositories.ReservationRepo;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _context;

    public ReservationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Reservation> GetUserReservations(string userId)
    {
        return _context.Reservations.Where(r => r.UserId == userId).ToList();
    }
}
