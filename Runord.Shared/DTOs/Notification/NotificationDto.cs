using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.Notification
{
    // DTO для передачи данных уведомления
    public record NotificationDto(
        Guid Id,
        Guid? UserId,
        NotificationType Type,
        string Title,
        string Message,
        bool IsRead,
        DateTimeOffset CreatedAt
    ) : BaseDto(Id);
}
