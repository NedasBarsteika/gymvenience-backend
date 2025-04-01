using gymvenience.Services.ReservationService;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetUserReservations(int userId)
    {
        var reservations = _reservationService.GetUserReservations(userId);
        if (!reservations.Any())
        {
            return NotFound("No reservations found for this user.");
        }
        return Ok(reservations);
    }
}
