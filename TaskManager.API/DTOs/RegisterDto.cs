using System.ComponentModel.DataAnnotations;

namespace Task_Manager_API.DTOs
{
    public record RegisterDto(
        [Required][MaxLength(50)] string Username,
        [Required][MaxLength(6)] string Password
        );
}
