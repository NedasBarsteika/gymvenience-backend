using gymvenience.Models;
using gymvenience.Services.GymService;
using gymvenience_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gymvenience.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GymController : ControllerBase
    {
        private readonly IGymService _gymService;

        public GymController(IGymService gymService)
        {
            _gymService = gymService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gym>>> GetAllGyms()
        {
            var gyms = await _gymService.GetAllGymsAsync();
            return Ok(gyms);
        }

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _gymService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            var addresses = await _gymService.GetAllAddressesAsync();
            return Ok(addresses);
        }

        [HttpGet("{id}/summary")]
        public IActionResult GetGymSummary(string id)
        {
            var summary = _gymService.GetGymSummaryById(id);
            if (summary == null)
                return NotFound("Gym not found.");

            return Ok(summary);
        }
    }
}
