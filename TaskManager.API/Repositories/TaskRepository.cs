using Microsoft.EntityFrameworkCore;
using Task_Manager_API.DTOs;
using Task_Manager_API.Models;
using Task_Manager_API.Pagination;
using Task_Manager_API.Persistence;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Task_Manager_API.Repositories
{
    public class TaskRepository(TaskDbContext context) : ITaskRepository
    {
        public async Task AddAsync(TaskItem task)
        {
            await context.Tasks.AddAsync(task);
        }

        public Task DeleteAsync(TaskItem task)
        {
            context.Tasks.Remove(task);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await context.Tasks.AnyAsync(t => t.Id == id);
        }

        public async Task<PagedResult<TaskItem>> GetAllAsync(PaginationParams pagination)
        {
            var query = context.Tasks.AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(t => t.Id)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<TaskItem>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public Task UpdateAsync(TaskItem task)
        {
            context.Tasks.Update(task);
            return Task.CompletedTask;
        }
    }
}
