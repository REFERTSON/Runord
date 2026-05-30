using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Project;
using Runord.Shared.Interfaces;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // Получение списка проектов (только свои для User, все для Admin)
        [HttpGet]
        public async Task<ActionResult<Result<PagedResponse<ProjectDto>>>> GetProjects(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var result = await _projectService.GetProjectsAsync(userId, isAdmin, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Result<ProjectDto>>> GetProject(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var result = await _projectService.GetProjectByIdAsync(id, userId, isAdmin);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Result<ProjectDto>>> CreateProject([FromBody] CreateProjectRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _projectService.CreateProjectAsync(request, userId);
            return CreatedAtAction(nameof(GetProject), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result<ProjectDto>>> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var result = await _projectService.UpdateProjectAsync(id, request, userId, isAdmin);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<bool>>> DeleteProject(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var result = await _projectService.DeleteProjectAsync(id, userId, isAdmin);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
    }
}
