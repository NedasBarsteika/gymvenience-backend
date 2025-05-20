using gymvenience.Services.TrainerAvailabilityService;
using gymvenience_backend.DTOs;
using gymvenience_backend.Repositories.UserRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TrainerAvailabilityController : ControllerBase
{
    private readonly ITrainerAvailabilityService _service;
    private readonly IUserRepository _userRepo;

    public TrainerAvailabilityController(ITrainerAvailabilityService service, IUserRepository userRepo)
    {
        _service = service;
        _userRepo = userRepo;
    }

    // POST api/traineravailability
    //[Authorize(Roles = "Trainer")]
    [HttpPost]
    public IActionResult AddSlot([FromBody] AvailabilityDto dto)
    {
        var trainer = _userRepo.GetByIdAsync(dto.TrainerId);
        if (trainer == null)
            return NotFound($"Trainer with id {dto.TrainerId} not found.");

        var gymId = trainer.Result.Gym.Id;

        _service.AddSlot(dto.TrainerId, dto.Date, dto.StartTime, dto.Duration, gymId);
        return Ok();
    }

    // GET api/traineravailability/{trainerId}?date=2025-05-05
    [HttpGet("{trainerId}/all")]
    public IActionResult GetAllSlots(string trainerId)
    {
        var slots = _service.GetAllTrainerSlots(trainerId);
        return Ok(slots);
    }

    // GET api/traineravailability/{trainerId}?date=2025-05-05
    [HttpGet("{trainerId}/available")]
    public IActionResult GetAvailableSlots(string trainerId, [FromQuery] DateTime date)
    {
        var slots = _service.GetAvailableSlotsForDate(trainerId, date);
        return Ok(slots);
    }
    // DELETE api/traineravailability/{slotId}
    //[Authorize(Roles = "Trainer")]
    [HttpDelete("{slotId}")]
    public IActionResult DeleteSlot(string slotId)
    {
        var success = _service.DeleteSlot(slotId);
        if (!success) return NotFound();
        return NoContent();
    }
    // PUT api/traineravailability/{slotId}
    //[Authorize(Roles = "Trainer")]
    [HttpPut("{slotId}")]
    public IActionResult UpdateSlot(string slotId, [FromBody] UpdateTrainerSlotDto dto)
    {
        var updated = _service.UpdateSlot(slotId, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

}
