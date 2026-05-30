using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;

namespace Runord.Shared.Interfaces
{
    public interface INotificationService
    {
        // Чтение
        Task<Result<PagedResponse<NotificationDto>>> GetUserNotificationsAsync(
            Guid userId,
            NotificationFilter filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        // Создание (одиночное и массовое)
        Task<Result<Guid>> CreateNotificationAsync(
            Guid userId,
            CreateNotificationRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<int>> BroadcastNotificationAsync(
            IEnumerable<Guid> userIds,
            CreateNotificationRequest request,
            CancellationToken cancellationToken = default);

        // Обновление (одиночное и массовое)
        Task<Result<bool>> MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Result<int>> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        // Удаление (одиночное и массовое)
        Task<Result<bool>> DeleteNotificationAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<Result<int>> DeleteAllUserNotificationsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}