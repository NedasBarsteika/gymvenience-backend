using AutoMapper;
using gymvenience_backend.DTOs;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
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
        public async Task<ActionResult<string>> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            string email = userLoginDto.Email;
            string password = userLoginDto.Password;


            var (res, jwtToken) = await _userService.GenerateJwtAsync(email, password);
            if (!res.IsSuccess)
            {
                return BadRequest(res.Message);
            }

            return Ok(jwtToken);
        }
    }
}
