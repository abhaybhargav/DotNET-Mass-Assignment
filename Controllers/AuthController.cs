using Microsoft.AspNetCore.Mvc;
using VulnerableAPI.Models;
using VulnerableAPI.Services;

namespace VulnerableAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] User user)
        {
            if (_userService.GetUser(user.Email) != null)
            {
                return BadRequest("User already exists");
            }

            if (_userService.AddUser(user))
            {
                return Ok("User created successfully");
            }

            return BadRequest("Failed to create user");
        }

        [HttpPost("signup-secure")]
        public IActionResult SignUpSecure([FromBody] UserDto userDto)
        {
            if (_userService.GetUser(userDto.Email) != null)
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Email = userDto.Email,
                Name = userDto.Name,
                Password = userDto.Password,
                IsAdmin = false // Always set to false for secure signup
            };

            if (_userService.AddUser(user))
            {
                return Ok("User created successfully");
            }

            return BadRequest("Failed to create user");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto userDto)
        {
            var user = _userService.GetUser(userDto.Email);

            if (user == null || user.Password != userDto.Password)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _userService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}