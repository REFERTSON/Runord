using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Runord.Hub.Services;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Interfaces.Services;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<Result<PagedResponse<TaskDto>>>> GetTasks(
            [FromQuery] Guid? projectId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // Извлекаем строковый Id пользователя из Claim-ов
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Безопасно парсим string в Guid?, как того требует сигнатура метода GetTasksAsync[cite: 4, 5]
            Guid? userId = Guid.TryParse(userIdStr, out var parsedGuid) ? parsedGuid : null;
            var isAdmin = User.IsInRole("Admin");

            var result = await _taskService.GetTasksAsync(userId, isAdmin, projectId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Result<TaskDto>>> GetTask(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

        var isAdmin = User.IsInRole("Admin");
        var result = await _taskService.GetTaskByIdAsync(id, userId, isAdmin);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Result<TaskDto>>> CreateTask([FromBody] CreateTaskRequest request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Парсим string в Guid для метода CreateTaskAsync
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

    var result = await _taskService.CreateTaskAsync(request, userId);
            return CreatedAtAction(nameof(GetTask), new { id = result.Data?.Id }, result);
}

[HttpPut("{id:guid}/status")]
public async Task<ActionResult<Result<bool>>> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
{
    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            Guid? userId = Guid.TryParse(userIdStr, out var parsedGuid) ? parsedGuid : null;
    var isAdmin = User.IsInRole("Admin");

            var result = await _taskService.UpdateTaskStatusAsync(id, request.Status, userId, isAdmin);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

[HttpDelete("{id:guid}")]
public async Task<ActionResult<Result<bool>>> DeleteTask(Guid id)
{
    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Парсим string в Guid для будущего метода удаления
            if (!Guid.TryParse(userIdStr, out var userId))
        return Unauthorized();

    var isAdmin = User.IsInRole("Admin");
            
            // Вызываем асинхронный метод (убедитесь, что он добавлен в ITaskService и TaskService.cs)[cite: 5]
            var result = await _taskService.DeleteTaskAsync(id, userId, isAdmin);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}