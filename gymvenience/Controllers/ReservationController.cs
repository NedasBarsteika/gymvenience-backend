using gymvenience.Services.ReservationService;
using Microsoft.AspNetCore.Mvc;
using gymvenience_backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using gymvenience_backend.Models;
using Microsoft.EntityFrameworkCore;
using gymvenience_backend;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ApplicationDbContext _context;

    public ReservationController(IReservationService reservationService, ApplicationDbContext context)
    {
        _reservationService = reservationService;
        _context = context;
    }

    [HttpGet("{userId}")]
    public IActionResult GetUserReservations(string userId)
    {
        var reservations = _reservationService.GetUserReservations(userId);
        if (!reservations.Any())
        {
            return NotFound("No reservations found for this user.");
        }
        return Ok(reservations);
    }
    // POST api/reservations
    [HttpPost]
    public IActionResult BookReservation([FromBody] ReservationDto dto)
    {
        if (dto.UserId == null)
            return Unauthorized("No user ID in token.");
        var result = _reservationService.CreateReservation(dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Reservation);
    }
    // DELETE api/reservations/{reservationId}
    [HttpDelete("{reservationId}")]
    public IActionResult CancelReservation(string reservationId)
    {
        var result = _reservationService.CancelReservation(reservationId);
        if (!result)
            return BadRequest("Unable to cancel reservation. It may be too close to the session start.");

        return NoContent();
    }
    // POST api/reservation/{reservationId}/done
    [HttpPost("{reservationId}/done")]
    //[Authorize]
    public async Task<IActionResult> MarkDone(string reservationId)
    {
        var ok = await _reservationService.MarkDoneAsync(reservationId);
        if (!ok)
            return NotFound(new { message = "Reservation not found or already marked done." });
        return Ok(new { message = "Reservation marked as done." });
    }
    /// <summary>
    /// Get all reservations (booked slots) for a specific trainer.
    /// </summary>
    [HttpGet("trainer/{trainerId}")]
    //[Authorize(Roles = "Trainer,Admin")]
    public IActionResult GetTrainerReservations(string trainerId)
    {
        var reservations = _reservationService.GetReservationsForTrainer(trainerId);
        if (!reservations.Any())
            return NotFound("No reservations found for this trainer.");
        return Ok(reservations);
    }
    // GET api/reservation/all
    [HttpGet("all")]
    //[Authorize(Roles = "Admin")]
    public IActionResult GetAllReservations()
    {
        var reservations = _reservationService.GetAllReservations();
        return Ok(reservations);
    }

}
