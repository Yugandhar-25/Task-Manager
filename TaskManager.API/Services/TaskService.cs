using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Task_Manager_API.DTOs;
using Task_Manager_API.Models;
using Task_Manager_API.Pagination;
using Task_Manager_API.Persistence;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Task_Manager_API.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _context;
        private readonly ILogger<TaskService> _logger;
        public TaskService(TaskDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private static TaskItemDTO MaptoDto(TaskItem t) => 
            new(t.Id, t.Title, t.IsCompleted, t.CreatedAt, t.UpdatedAt);

        public async Task<PagedResult<TaskItemDTO>> GetAllAsync(PaginationParams pagination)
        {
            //return await _context.Tasks.AsNoTracking().Select(t => MaptoDto(t)).ToListAsync();
            _logger.LogInformation("Fetching tasks - Page: {Page}, PageSize: {PageSize}", pagination.Page, pagination.PageSize);

            var query = _context.Tasks.AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.Id)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(t => MaptoDto(t))
                .ToListAsync();

            _logger.LogInformation("Fetching {Count} tasks out of {Total}", items.Count, totalCount);

            return new PagedResult<TaskItemDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<TaskItemDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching task with id: {id}", id);
            var task = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

            if (task is null) _logger.LogWarning("Task with id:{id} was not found", id);
            return task == null ? null : MaptoDto(task);
        }

        public async Task<TaskItemDTO> CreateTaskAsync(CreateTaskDTO dto)
        {
            _logger.LogInformation("Creating task with title: {title}", dto.Title);
            var task = new TaskItem
            {
                Title = dto.Title,
                IsCompleted = false
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Task created sucessfully with id: {id}", task.Id);
            return MaptoDto(task);
        }

        public async Task<TaskItemDTO> UpdateTaskAsync(int id, UpdateTaskDTO dto)
        {
            _logger.LogInformation("Updating task with id: {id}", id);
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                _logger.LogWarning("Task with id: {Id} not found for update", id);
                return null;
            }
            task.Title = dto.Title;
            task.IsCompleted = dto.isCompleted;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Task updated sucessfully with id: {id}", id);
            return MaptoDto(task);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting task with id: {id}", id);
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                _logger.LogWarning("Task with id: {Id} not found for deletion", id);
                return false;
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Task deleted sucessfully with id: {id}", id);
            return true;
        }
    }
}
