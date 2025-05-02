using gymvenience.DTOs;
using gymvenience.Models;
using gymvenience.Services.ReservationService;
using gymvenience_backend.DTOs;
using gymvenience_backend.Repositories.ReservationRepo;
using Microsoft.EntityFrameworkCore;

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
    public ReservationResult CreateReservation(ReservationDto dto)
    {
        var slot = _reservationRepository.GetAvailableTimeSlot(dto.SlotId);
        if (slot == null)
            return new ReservationResult(false, "Time slot already booked or doesn't exist.", null);

        var gym = _reservationRepository.GetGymById(dto.GymId);
        if (gym == null)
            return new ReservationResult(false, "Gym not found.", null);

        var reservation = new Reservation
        {
            Id = Guid.NewGuid().ToString(),
            UserId = dto.UserId,
            TrainerId = slot.TrainerId,
            Date = slot.Date,
            Time = slot.StartTime,
            Duration = slot.Duration,
            GymId = dto.GymId
        };

        _reservationRepository.MarkTimeSlotReserved(slot);
        _reservationRepository.AddReservation(reservation);
        _reservationRepository.SaveChanges();

        return new ReservationResult(true, "Reservation successful", reservation);
    }

    public bool CancelReservation(string reservationId)
    {
        var reservation = _reservationRepository.GetReservationById(reservationId);
        if (reservation == null)
            return false;

        var startDateTime = reservation.Date.Date + reservation.Time;
        if ((startDateTime - DateTime.Now).TotalHours < 12)
            return false; // Too late to cancel

        var slot = _reservationRepository.GetTimeSlotByTrainerAndTime(reservation.TrainerId, reservation.Date, reservation.Time);
        if (slot != null)
        {
            slot.Reserved = false;
        }

        _reservationRepository.RemoveReservation(reservation);
        _reservationRepository.SaveChanges();

        return true;
    }
    public async Task<bool> MarkDoneAsync(string reservationId)
    {
        var reservation = _reservationRepository.GetReservationById(reservationId);
        if (reservation == null || reservation.IsDone)
            return false;

        reservation.IsDone = true;
        await _reservationRepository.SaveChangesAsync();
        return true;
    }
    public IEnumerable<Reservation> GetReservationsForTrainer(string trainerId)
    {
        return _reservationRepository.GetReservationsByTrainer(trainerId);
    }
}