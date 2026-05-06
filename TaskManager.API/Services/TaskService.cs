using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Task_Manager_API.DTOs;
using Task_Manager_API.Models;
using Task_Manager_API.Pagination;
using Task_Manager_API.Persistence;
using Task_Manager_API.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Task_Manager_API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<TaskService> _logger;
        public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private static TaskItemDTO MaptoDto(TaskItem t) => 
            new(t.Id, t.Title, t.IsCompleted, t.CreatedAt, t.UpdatedAt);

        public async Task<PagedResult<TaskItemDTO>> GetAllAsync(PaginationParams pagination)
        {
            _logger.LogInformation("Fetching tasks - Page: {Page}, PageSize: {PageSize}", pagination.Page, pagination.PageSize);
            var result = await _repository.GetAllAsync(pagination);
            _logger.LogInformation("Fetching {Count} tasks out of {Total}", result.Items.Count(), result.TotalCount);
            return new PagedResult<TaskItemDTO>
            {
                Items = result.Items.Select(MaptoDto),
                TotalCount = result.TotalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<TaskItemDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching task with id: {id}", id);
            var result = await _repository.GetByIdAsync(id);
            if (result is null) _logger.LogWarning("Task with id:{id} was not found", id);
            return result == null ? null : MaptoDto(result);
        }

        public async Task<TaskItemDTO> CreateTaskAsync(CreateTaskDTO dto)
        {
            _logger.LogInformation("Creating task with title: {title}", dto.Title);
            var task = new TaskItem
            {
                Title = dto.Title,
                IsCompleted = false
            };
            await _repository.AddAsync(task);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Task created sucessfully with id: {id}", task.Id);
            return MaptoDto(task);
        }

        public async Task<TaskItemDTO> UpdateTaskAsync(int id, UpdateTaskDTO dto)
        {
            _logger.LogInformation("Updating task with id: {id}", id);
            var task = await _repository.GetByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with id: {Id} not found for update", id);
                return null;
            }
            task.Title = dto.Title;
            task.IsCompleted = dto.isCompleted;
            task.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(task);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Task updated sucessfully with id: {id}", id);
            return MaptoDto(task);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting task with id: {id}", id);
            var task = await _repository.GetByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with id: {Id} not found for deletion", id);
                return false;
            }
            await _repository.DeleteAsync(task);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Task deleted sucessfully with id: {id}", id);
            return true;
        }
    }
}
