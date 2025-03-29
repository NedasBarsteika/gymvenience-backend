using AutoMapper;
using gymvenience_backend.DTOs;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public UserController(IUserService userService, IUserRepository userRepository, IMapper mapper)
        {
            _userService = userService;
            _userRepository = userRepository;
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
    }
}
