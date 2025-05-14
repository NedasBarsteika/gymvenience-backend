using FluentAssertions.Equivalency.Tracing;
using gymvenience.DTOs;
using gymvenience.Models;
using gymvenience.Services.ReservationService;
using gymvenience_backend.DTOs;
using gymvenience_backend.Repositories.ReservationRepo;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        var user = _reservationRepository.GetUserById(dto.UserId);
        if (user == null)
            return new ReservationResult(false, "User not found.", null);

        var trainer = _reservationRepository.GetUserById(slot.TrainerId);
        if (trainer == null || !trainer.IsTrainer)
            return new ReservationResult(false, "Trainer not found.", null);

        decimal rate = trainer.HourlyRate;

        var reservation = new Reservation
        {
            Id = Guid.NewGuid().ToString(),
            UserId = dto.UserId,
            TrainerId = slot.TrainerId,
            Date = slot.Date,
            Time = slot.StartTime,
            Duration = slot.Duration,
            GymId = dto.GymId,
            RateAtBooking = rate
        };

        _reservationRepository.MarkTimeSlotReserved(slot);
        _reservationRepository.AddReservation(reservation);
        user.Reservations.Add(reservation);
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
    public IEnumerable<Reservation> GetAllReservations()
    {
        return _reservationRepository.GetAllReservations();
    }

}