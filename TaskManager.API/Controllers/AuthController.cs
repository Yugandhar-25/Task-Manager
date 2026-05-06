using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Manager_API.DTOs;
using Task_Manager_API.Services;

namespace Task_Manager_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            try
            {
                var user = await authService.RegisterAsync(dto);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await authService.LoginAsync(dto);
            if (user is null) return Unauthorized("Invalid username/password");
            return Ok(user);
        }
    }
}

