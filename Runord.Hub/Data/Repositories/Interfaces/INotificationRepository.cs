using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Entities;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Data.Repositories.Interfaces
{
    public interface INotificationRepository : IBaseRepository<NotificationEntity>
    {
        // Пагинация + фильтрация (возвращает PagedResponse)
        Task<PagedResponse<NotificationEntity>> GetPagedUserNotificationsAsync(
            Guid userId,
            NotificationFilter filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        // Массовое обновление (отметить все как прочитанные)
        Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);

        // Массовое удаление всех уведомлений пользователя
        Task<int> DeleteAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);

        // Добавление нескольких записей (для массовой рассылки)
        Task AddRangeAsync(IEnumerable<NotificationEntity> entities, CancellationToken cancellationToken = default);
    }
}