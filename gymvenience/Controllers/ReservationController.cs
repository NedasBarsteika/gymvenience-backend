using gymvenience.Services.ReservationService;
using Microsoft.AspNetCore.Mvc;
using gymvenience_backend.DTOs;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
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
    [Authorize]
    [HttpPost]
    public IActionResult BookReservation([FromBody] ReservationDto dto)
    {
        var result = _reservationService.CreateReservation(dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Reservation);
    }
    // DELETE api/reservations/{reservationId}
    [Authorize]
    [HttpDelete("{reservationId}")]
    public IActionResult CancelReservation(string reservationId)
    {
        var result = _reservationService.CancelReservation(reservationId);
        if (!result)
            return BadRequest("Unable to cancel reservation. It may be too close to the session start.");

        return NoContent();
    }
}
