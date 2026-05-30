using Runord.Shared.Base;
using Runord.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.DTOs.Notification
{
    public record NotificationDto(
        Guid Id,
        Guid? UserId,
        NotificationType Type,
        string Title,
        string Message,
        bool IsRead,
        DateTimeOffset CreatedAt
    ) : BaseDto(Id);

    public record CreateNotificationRequest(
        Guid? UserId,
        NotificationType Type,
        string Title,
        string Message
    );

    public record UpdateNotificationRequest(
        Guid Id,
        bool IsRead
    );

    public record DeleteNotificationRequest(Guid Id);
}
