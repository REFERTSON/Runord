using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;
using System.ComponentModel;
using System.Security.Claims;

namespace Runord.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Description("Управление уведомлениями текущего пользователя.")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
            => _notificationService = notificationService;

        [HttpGet]
        [EndpointSummary("Получить список уведомлений")]
        [EndpointDescription("Возвращает уведомления текущего пользователя с возможностью фильтрации.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<IEnumerable<NotificationDto>>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<IEnumerable<NotificationDto>>>> GetNotifications(
            [FromQuery] NotificationFilter? filter)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized("Неверный идентификатор пользователя");
            var result = await _notificationService.GetNotificationsAsync(userId.Value, filter);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/read")]
        [EndpointSummary("Отметить уведомление как прочитанное")]
        [EndpointDescription("Помечает одно уведомление прочитанным.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<bool>>> MarkAsRead(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.MarkAsReadAsync(userId.Value, id);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPatch("read-all")]
        [EndpointSummary("Отметить все уведомления как прочитанные")]
        [EndpointDescription("Помечает все уведомления пользователя прочитанными.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<int>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<int>>> MarkAllAsRead()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.MarkAllAsReadAsync(userId.Value);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [EndpointSummary("Удалить одно уведомление")]
        [EndpointDescription("Удаляет указанное уведомление пользователя.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<bool>>> DeleteNotification(Guid id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();
            var result = await _notificationService.DeleteNotificationAsync(userId.Value, id);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("all")]
        [EndpointSummary("Удалить все уведомления")]
        [EndpointDescription("Удаляет все уведомления текущего пользователя.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<int>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Response<int>>> DeleteAllNotifications()
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
}