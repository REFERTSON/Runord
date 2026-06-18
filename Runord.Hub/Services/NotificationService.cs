using Mapster;
using Microsoft.AspNetCore.SignalR;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Hubs;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Entities;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;

namespace Runord.Hub.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        // ---------------------- Чтение ----------------------
        public async Task<Response<IEnumerable<NotificationDto>>> GetNotificationsAsync(
            Guid userId,
            NotificationFilter? filter,
            CancellationToken cancellationToken = default)
        {
            filter ??= new NotificationFilter();
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId, filter, cancellationToken);
            return Response<IEnumerable<NotificationDto>>.Success(notifications.Adapt<List<NotificationDto>>());
        }

        // ---------------------- Создание (только система) ----------------------
        public async Task<Response<Guid>> CreateNotificationAsync(
            NotificationEntity notification,
            CancellationToken cancellationToken = default)
        {
            if (notification.UserId == Guid.Empty)
                return Response<Guid>.Failure("UserId не может быть пустым");

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = notification.Adapt<NotificationDto>();
            await _hubContext.Clients.Group($"user_{notification.UserId}_notifications")
                .SendAsync("NewNotification", dto, cancellationToken);

            return Response<Guid>.Success(notification.Id);
        }

        // ---------------------- Обновление ----------------------
        public async Task<Response<bool>> MarkAsReadAsync(
            Guid userId,
            Guid notificationId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
            if (notification == null || notification.UserId != userId)
                return Response<bool>.Failure("Уведомление не найдено или доступ запрещён");

            notification.IsRead = true;
            notification.LastModified = DateTimeOffset.UtcNow;
            _notificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = notification.Adapt<NotificationDto>();
            await _hubContext.Clients.Group($"user_{userId}_notifications")
                .SendAsync("NotificationUpdated", dto, cancellationToken);

            return Response<bool>.Success(true);
        }

        public async Task<Response<int>> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var updatedCount = await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
            if (updatedCount > 0)
            {
                await _hubContext.Clients.Group($"user_{userId}_notifications")
                    .SendAsync("AllNotificationsRead", cancellationToken);
            }
            return Response<int>.Success(updatedCount);
        }

        // ---------------------- Удаление ----------------------
        public async Task<Response<bool>> DeleteNotificationAsync(
            Guid userId,
            Guid notificationId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
            if (notification == null || notification.UserId != userId)
                return Response<bool>.Failure("Уведомление не найдено или доступ запрещён");

            _notificationRepository.Delete(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _hubContext.Clients.Group($"user_{userId}_notifications")
                .SendAsync("NotificationDeleted", notificationId, cancellationToken);

            return Response<bool>.Success(true);
        }

        public async Task<Response<int>> DeleteAllUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var deletedCount = await _notificationRepository.DeleteAllForUserAsync(userId, cancellationToken);
            if (deletedCount > 0)
            {
                await _hubContext.Clients.Group($"user_{userId}_notifications")
                    .SendAsync("AllNotificationsDeleted", cancellationToken);
            }
            return Response<int>.Success(deletedCount);
        }
    }
}