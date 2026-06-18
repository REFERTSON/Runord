using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Project;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;
using System.ComponentModel;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Description("Управление проектами пользователя.")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }

        [HttpGet]
        [EndpointSummary("Получить проекты пользователя")]
        [EndpointDescription("Возвращает список проектов, доступных текущему пользователю.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<IEnumerable<ProjectDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<IEnumerable<ProjectDto>>>> GetProjects([FromQuery] ProjectFilter? filter)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _projectService.GetProjectsAsync(userId, filter);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Получить проект по ID")]
        [EndpointDescription("Возвращает详细信息 проекта, если у пользователя есть доступ.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ProjectDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<ProjectDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<ProjectDto>>> GetProject(Guid id)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _projectService.GetProjectByIdAsync(id, userId);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [EndpointSummary("Создать проект")]
        [EndpointDescription("Создает новый проект для текущего пользователя.")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<ProjectDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<ProjectDto>>> CreateProject([FromBody] CreateProjectRequest request)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _projectService.CreateProjectAsync(request, userId);
            if (!result.IsSuccess) return BadRequest(result);
            return CreatedAtAction(nameof(GetProject), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        [EndpointSummary("Обновить проект")]
        [EndpointDescription("Обновляет данные проекта (только владелец или администратор).")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ProjectDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<ProjectDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<ProjectDto>>> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var result = await _projectService.UpdateProjectAsync(id, request, userId, isAdmin);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [EndpointSummary("Удалить проект")]
        [EndpointDescription("Удаляет проект (только владелец или администратор).")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<bool>>> DeleteProject(Guid id)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var result = await _projectService.DeleteProjectAsync(id, userId, isAdmin);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
    }
}