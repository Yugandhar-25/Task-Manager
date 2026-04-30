using System.ComponentModel.DataAnnotations;

namespace Task_Manager_API.DTOs
{
    public record LoginDto(
        [Required] string Username,
        [Required] string Password);
}
