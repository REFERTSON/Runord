using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Filters;

namespace Runord.Shared.Interfaces
{
    public interface INotificationService
    {
        // Метод для получения уведомлений пользователя с поддержкой фильтрации
        Task<Response<IEnumerable<NotificationDto>>> GetNotificationsAsync(
            Guid userId,
            NotificationFilter? filter = null,
            CancellationToken cancellationToken = default);

        // Методы для управления статусом уведомлений (отметить как прочитанное - одиночное и массовое)
        Task<Response<bool>> MarkAsReadAsync(
            Guid userId,
            Guid notificationId,
            CancellationToken cancellationToken = default);

        Task<Response<int>> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        // Методы для удаления уведомлений (одиночное и массовое)
        Task<Response<bool>> DeleteNotificationAsync(
            Guid userId,
            Guid notificationId,
            CancellationToken cancellationToken = default);

        Task<Response<int>> DeleteAllUserNotificationsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}