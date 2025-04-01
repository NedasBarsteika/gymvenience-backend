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
        [HttpPut("me")]
        public IActionResult UpdateMyProfile([FromBody] UpdateProfileDto updateDto)
        {
            // Once again, we identify the user from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            return await _context.Users.Where(u => u.IsTrainer).ToListAsync();
        }
    }

    public class UpdateProfileDto
    {
        public string Bio { get; set; }
    }
}
