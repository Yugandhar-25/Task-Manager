namespace Task_Manager_API.DTOs
{
    public record TaskItemDTO(int id, string title, bool isCompleted, DateTime createdAt, DateTime? updatedAt);

}
