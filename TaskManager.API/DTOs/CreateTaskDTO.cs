using System.ComponentModel.DataAnnotations;

namespace Task_Manager_API.DTOs
{
    public record CreateTaskDTO([Required][MaxLength(200)] string Title);
}
