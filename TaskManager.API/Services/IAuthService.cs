using Task_Manager_API.DTOs;

namespace Task_Manager_API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    }
}
