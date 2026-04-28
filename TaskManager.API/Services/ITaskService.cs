using Task_Manager_API.DTOs;
using Task_Manager_API.Models;
using Task_Manager_API.Pagination;

namespace Task_Manager_API.Services
{
    public interface ITaskService
    {
        Task<PagedResult<TaskItemDTO>> GetAllAsync(PaginationParams pagination);
        Task<TaskItemDTO?> GetByIdAsync(int id);
        Task<TaskItemDTO> CreateTaskAsync(CreateTaskDTO dto);
        Task<TaskItemDTO> UpdateTaskAsync(int id, UpdateTaskDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
