using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces.Services;
using System.ComponentModel;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Description("Управление задачами в рамках проектов.")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService) => _taskService = taskService;

        private Guid GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        [HttpGet]
        [EndpointSummary("Получить список задач")]
        [EndpointDescription("Возвращает задачи с учётом фильтрации и прав пользователя.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<IEnumerable<TaskDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<IEnumerable<TaskDto>>>> GetTasks([FromQuery] TaskFilter filter)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            var result = await _taskService.GetTasksAsync(filter ?? new TaskFilter(), userId, isAdmin);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Получить задачу по ID")]
        [EndpointDescription("Возвращает详细信息 задачи, если доступ разрешён.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<TaskDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<TaskDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<TaskDto>>> GetTask(Guid id)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            var result = await _taskService.GetTaskByIdAsync(id, userId, isAdmin);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [EndpointSummary("Создать новую задачу")]
        [EndpointDescription("Создаёт задачу и связывает её с проектом.")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<TaskDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<TaskDto>>> CreateTask([FromBody] CreateTaskRequest request)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();
            var result = await _taskService.CreateTaskAsync(request, userId);
            if (!result.IsSuccess) return BadRequest(result);
            return CreatedAtAction(nameof(GetTask), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}/status")]
        [EndpointSummary("Изменить статус задачи")]
        [EndpointDescription("Обновляет статус задачи. Доступно владельцу, исполнителю или администратору.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<bool>>> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");
            var result = await _taskService.UpdateTaskStatusAsync(id, request.Status, userId == Guid.Empty ? null : userId, isAdmin);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [EndpointSummary("Удалить задачу")]
        [EndpointDescription("Удаляет задачу. Доступно автору, владельцу проекта или администратору.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<bool>>> DeleteTask(Guid id)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            var result = await _taskService.DeleteTaskAsync(id, userId, isAdmin);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
    }
}