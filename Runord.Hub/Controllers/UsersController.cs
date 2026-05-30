using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.User;
using Runord.Shared.Interfaces;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) => _userService = userService;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<Result<PagedResponse<UserDto>>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _userService.GetUsersAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<ActionResult<Result<UserDto>>> GetCurrentUser()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _userService.GetUserByIdAsync(userId.Value);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Result<UserDto>>> GetUser(Guid id)
        {
            var currentUserId = GetUserId();
            if (!currentUserId.HasValue) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id) return Forbid();
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Result<UserDto>>> CreateUser(CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            if (!result.IsSuccess) return BadRequest(result);
            return CreatedAtAction(nameof(GetUser), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result<UserDto>>> UpdateUser(Guid id, UpdateUserRequest request)
        {
            var currentUserId = GetUserId();
            if (!currentUserId.HasValue) return Unauthorized();
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && currentUserId != id) return Forbid();
            var result = await _userService.UpdateUserAsync(id, request);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<bool>>> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPatch("me/email")]
        public async Task<ActionResult<Result<bool>>> ChangeEmail(ChangeEmailRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _userService.ChangeEmailAsync(userId.Value, request);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("me/avatar")]
        public async Task<ActionResult<Result<bool>>> UpdateAvatar(UpdateAvatarRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _userService.UpdateAvatarAsync(userId.Value, request);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:guid}/block")]
        public async Task<ActionResult<Result<bool>>> BlockUser(Guid id, BlockUserRequest request)
        {
            var result = await _userService.BlockUserAsync(id, request);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:guid}/unblock")]
        public async Task<ActionResult<Result<bool>>> UnblockUser(Guid id)
        {
            var result = await _userService.UnblockUserAsync(id);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }
}