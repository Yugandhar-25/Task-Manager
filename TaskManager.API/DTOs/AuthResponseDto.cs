namespace Task_Manager_API.DTOs
{
    public record AuthResponseDto(
        string Token, string Username, DateTime ExpiresAt);
}
