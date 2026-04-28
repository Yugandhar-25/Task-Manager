using System.ComponentModel.DataAnnotations;

namespace Task_Manager_API.DTOs
{
    public record UpdateTaskDTO([Required] [MaxLength(200)] string Title, bool isCompleted);
}
