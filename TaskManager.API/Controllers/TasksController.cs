using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Manager_API.DTOs;
using Task_Manager_API.Pagination;
using Task_Manager_API.Services;

namespace Task_Manager_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetAll([FromQuery]PaginationParams pagination)
        {
            var tasks = await taskService.GetAllAsync(pagination);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDTO>> GetTaskById(int id)
        {
            var task = await taskService.GetByIdAsync(id);
            return task == null ? NotFound() : Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDTO>> CreateTask(CreateTaskDTO dto)
        {
            var task = await taskService.CreateTaskAsync(dto);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.id }, task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskItemDTO>> UpdateTask(int id, UpdateTaskDTO dto)
        {
            var task = await taskService.UpdateTaskAsync(id, dto);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var task = await taskService.DeleteAsync(id);
            return task ? NoContent() : NotFound();
        }
    }
}
