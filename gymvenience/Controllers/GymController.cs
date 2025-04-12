using gymvenience.Services.GymService;
using Microsoft.AspNetCore.Mvc;

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
    }
}
