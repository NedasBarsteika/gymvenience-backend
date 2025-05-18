using AutoMapper;
using gymvenience_backend.DTOs;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using gymvenience_backend.Models;
using Microsoft.EntityFrameworkCore;
using gymvenience.DTOs;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public UserController(IUserService userService, IUserRepository userRepository, ApplicationDbContext context, IMapper mapper)
        {
            _userService = userService;
            _userRepository = userRepository;
            _context = context;
        }

        [HttpPost("register", Name = "RegisterNewUser")]
        public async Task<ActionResult<string>> RegisterNewUser([FromBody] UserRegisterDto userRegisterDto)
        {
            string name = userRegisterDto.Name;
            string surname = userRegisterDto.Surname;
            string email = userRegisterDto.Email;
            string password = userRegisterDto.Password;

            var (result, user) = await _userService.CreateUserAsync(name, surname, email, password);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            var (res, jwtToken) = await _userService.GenerateJwtAsync(email, password);
            if (!res.IsSuccess)
            {
                return BadRequest(res.Message);
            }

            return Ok(jwtToken);
        }

        [HttpPost("login", Name = "LoginUser")]
        public async Task<ActionResult<object>> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            string email = userLoginDto.Email;
            string password = userLoginDto.Password;

            var (res, jwtToken) = await _userService.GenerateJwtAsync(email, password);
            if (!res.IsSuccess)
            {
                return BadRequest(res.Message);
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            return Ok(new
            {
                Token = jwtToken,
                User = user
            });
        }

        // GET /api/user/me
        [HttpGet("me")]
        public IActionResult GetMyProfile()
        {
            // Token validation means we have a ClaimsPrincipal in HttpContext.User
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("No user ID in token.");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            // Return the user data (e.g., an object with id, bio, etc.)
            return Ok(new
            {
                id = user.Id,
                bio = user.Bio
            });
        }

        // PUT /api/user/me
        [HttpPut("{userId}/me")]
        public IActionResult UpdateMyProfile([FromBody] UpdateProfileDto updateDto, string userId)
        {
            if (userId == null)
                return Unauthorized("No user ID in token.");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            // Update fields
            user.Bio = updateDto.Bio;

            // Save changes
            _context.SaveChanges();

            // Return the updated user
            return Ok(new
            {
                id = user.Id,
                bio = user.Bio
            });
        }

        [HttpGet("trainers")]
        public async Task<ActionResult<IEnumerable<User>>> GetTrainers()
        {
            return await _context.Users
                .Where(u => u.IsTrainer)
                .Include(u => u.Gym)
                .ToListAsync();
        }

        [HttpPost("{userId}/assign-gym/{gymId}")]
        public async Task<IActionResult> AssignGymToUser(string userId, string gymId)
        {
            var user = await _context.Users
                .Include(u => u.Gym)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            var gym = await _context.Gyms.FindAsync(gymId);
            if (gym == null)
                return NotFound("Gym not found");

            user.Gym = gym;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Gym assigned successfully", user.Id, Gym = gym });
        }

        /// <summary>
        /// Delete a user (by any role) from the system.
        /// </summary>
        /// <param name="id">The user’s unique ID.</param>
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]  // only admins can delete accounts
        public async Task<IActionResult> DeleteUser(string id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(new { message = "User not found." });

            return NoContent();
        }

        [HttpGet("searchTrainers", Name = "SearchTrainers")]
        public async Task<IEnumerable<User>> SearchTrainers(
        [FromQuery] string? city = null,
        [FromQuery] string? address = null)
        {
            city ??= "";
            address ??= "";

            var trainers = await _context.Users
                .Where(u => u.IsTrainer)
                .Include(u => u.Gym)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(city))
                trainers = trainers.Where(u => u.Gym != null && u.Gym.City.ToLower().Equals(city.ToLower())).ToList();

            if (!string.IsNullOrWhiteSpace(address))
                trainers = trainers.Where(u => u.Gym != null && u.Gym.Address.ToLower().Equals(address.ToLower())).ToList();

            return trainers;
        }
        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Demote a trainer back to a regular user and remove their availability slots.
        /// </summary>
        /// <param name="id">The trainer’s user ID.</param>
        [HttpPost("{id}/demote")]
        //[Authorize(Roles = "Admin")] // or whichever role is allowed
        public async Task<IActionResult> DemoteTrainer(string id)
        {
            var ok = await _userService.DemoteTrainerAsync(id);
            if (!ok) return NotFound(new { message = "Trainer not found or already demoted." });
            return Ok(new { message = "Trainer demoted successfully." });
        }

        /// <summary>
        /// Promote user to trainer.
        /// </summary>
        [HttpPost("{id}/promote")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToTrainer(string id)
        {
            var success = await _userService.PromoteToTrainerAsync(id);
            if (!success)
                return NotFound(new { message = "User not found or already a trainer." });

            return Ok(new { message = "User promoted to trainer." });
        }

        /// <summary>
        /// Get a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUserById(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Gym)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPost("{userId}/upload-image")]
        public async Task<IActionResult> UploadTrainerImage(
            [FromRoute] string userId,
            IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file uploaded.");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var savePath = Path.Combine("wwwroot", "Images", "Trainers", fileName);

            // Užtikrinkite, kad direktorija egzistuoja
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

            using var stream = new FileStream(savePath, FileMode.Create);
            await image.CopyToAsync(stream);

            // Grąžiname filename ir (tinkamu atveju) publiką url
            var publicUrl = $"{Request.Scheme}://{Request.Host}/Images/Trainers/{fileName}";
            return Ok(new { filename = fileName, url = publicUrl });
        }


        /// <summary>
        /// Atnaujina vartotojo (trenerio) profilio nuotraukos URL.
        /// </summary>
        /// <param name="userId">Vartotojo ID path’e</param>
        /// <param name="updateDto">DTO su ImageUrl</param>
        [HttpPut("{userId}/image")]
        public async Task<IActionResult> UpdateProfileImage(
            [FromRoute] string userId,
            [FromBody] UploadImageDto updateDto)
        {
            // Patikriname, ar toks vartotojas egzistuoja
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "Vartotojas nerastas." });

            // Atnaujiname ImageUrl lauką
            user.ImageUrl = updateDto.ImageUrl;

            // Įrašome pakeitimus
            await _context.SaveChangesAsync();

            // Grąžiname atnaujintą URL
            return Ok(new
            {
                user.Id,
                user.ImageUrl
            });

        [HttpGet("searchByName")]
        public async Task<ActionResult<IEnumerable<TrainerSummaryDto>>> SearchTrainersByName([FromQuery(Name = "q")] string? q = null)
        {
            var searchText = string.IsNullOrWhiteSpace(q) ? "" : q;

            var trainers = await _userService.SearchTrainersByNameAsync(searchText);
            if (!trainers.Any())
                return NotFound(new { message = "No matching trainers found." });

            var results = trainers.Select(u => new TrainerSummaryDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Bio = u.Bio,
                ImageUrl = u.ImageUrl,
                Rating = u.Rating,
                GymName = u.Gym?.Name,
                GymAddress = u.Gym?.Address
            });

            return Ok(results);
        }
        [HttpPut("{trainerid}/earnings")]
        //[Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> GetEarnings(string trainerid)
        {
            try
            {
                var amount = await _userService.CalculateTrainerEarningsAsync(trainerid);
                return Ok(new { trainerId = trainerid, earnings = amount });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Trainer not found.");
            }
        }

        [HttpPut("{trainerid}/hourlyRate")]
        //[Authorize(Roles = "Trainer,Admin")]
        public async Task<IActionResult> UpdateHourlyRate(string trainerid, [FromBody] decimal newRate)
        {
            var ok = await _userService.SetHourlyRateAsync(trainerid, newRate);
            if (!ok)
                return BadRequest("Cannot change rate while there are pending reservations.");
            return NoContent();
        }

    }

    public class UpdateProfileDto
    {
        public string Bio { get; set; }
    }
}
