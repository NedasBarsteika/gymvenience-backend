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
    public TrainerAvailability? GetAvailableTimeSlot(string slotId)
    {
        return _context.TrainerAvailabilities.FirstOrDefault(s => s.Id == slotId && !s.Reserved);
    }

    public void MarkTimeSlotReserved(TrainerAvailability slot)
    {
        slot.Reserved = true;
        _context.TrainerAvailabilities.Update(slot);
    }

    public Gym? GetGymById(string gymId)
    {
        return _context.Gyms.FirstOrDefault(g => g.Id == gymId);
    }

    public void AddReservation(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
    }

    public Reservation? GetReservationById(string reservationId)
    {
        return _context.Reservations.FirstOrDefault(r => r.Id == reservationId);
    }

    public void RemoveReservation(Reservation reservation)
    {
        _context.Reservations.Remove(reservation);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public TrainerAvailability? GetTimeSlotByTrainerAndTime(string trainerId, DateTime date, TimeSpan startTime)
    {
        return _context.TrainerAvailabilities
            .FirstOrDefault(s => s.TrainerId == trainerId && s.Date.Date == date.Date && s.StartTime == startTime);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public IEnumerable<Reservation> GetReservationsByTrainer(string trainerId)
    {
        return _context.Reservations
            .Where(r => r.TrainerId == trainerId)
            .ToList();
    }
    public IEnumerable<Reservation> GetAllReservations()
    {
        return _context.Reservations.ToList();
    }


}
