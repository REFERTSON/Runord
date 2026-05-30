using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Interfaces;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
            => _notificationService = notificationService;

        // 1. GET: получение с фильтрацией и пагинацией
        [HttpGet]
        public async Task<ActionResult<Result<PagedResponse<NotificationDto>>>> GetNotifications(
            [FromQuery] NotificationFilter filter,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized("Неверный идентификатор пользователя");
            var result = await _notificationService.GetUserNotificationsAsync(userId.Value, filter ?? new NotificationFilter(), page, pageSize);
            return Ok(result);
        }

        // 2. POST: создание личного уведомления
        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.CreateNotificationAsync(userId.Value, request);
            return Ok(result);
        }

        // 3. POST: массовая рассылка (только администратор)
        [HttpPost("broadcast")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<int>>> BroadcastNotifications([FromBody] BroadcastNotificationRequest request)
        {
            var result = await _notificationService.BroadcastNotificationAsync(request.UserIds, request.Notification);
            return Ok(result);
        }

        // 4. PATCH: отметить одно как прочитанное
        [HttpPatch("{id:guid}/read")]
        public async Task<ActionResult<Result<bool>>> MarkAsRead(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.MarkAsReadAsync(id, userId.Value);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        // 5. PATCH: отметить все как прочитанные
        [HttpPatch("read-all")]
        public async Task<ActionResult<Result<int>>> MarkAllAsRead()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.MarkAllAsReadAsync(userId.Value);
            return Ok(result);
        }

        // 6. DELETE: удалить одно уведомление
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<bool>>> DeleteNotification(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.DeleteNotificationAsync(id, userId.Value);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        // 7. DELETE: удалить все уведомления пользователя
        [HttpDelete("all")]
        public async Task<ActionResult<Result<int>>> DeleteAllNotifications()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.DeleteAllUserNotificationsAsync(userId.Value);
            return Ok(result);
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public class BroadcastNotificationRequest
    {
        public List<Guid> UserIds { get; set; } = new();
        public CreateNotificationRequest Notification { get; set; } = null!;
    }
}