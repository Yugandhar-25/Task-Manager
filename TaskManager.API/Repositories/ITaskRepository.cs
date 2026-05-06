using Task_Manager_API.DTOs;
using Task_Manager_API.Models;
using Task_Manager_API.Pagination;

namespace Task_Manager_API.Repositories
{
    public interface ITaskRepository
    {
        Task<PagedResult<TaskItem>> GetAllAsync(PaginationParams pagination);
        Task<TaskItem?> GetByIdAsync(int id);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
