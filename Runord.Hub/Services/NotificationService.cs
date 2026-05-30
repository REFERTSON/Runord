using Mapster;
using Microsoft.AspNetCore.SignalR;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Hubs;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Notification;
using Runord.Shared.Entities;
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
        public async Task<Result<PagedResponse<NotificationDto>>> GetUserNotificationsAsync(
            Guid userId,
            NotificationFilter filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            filter ??= new NotificationFilter();

            var pagedEntities = await _notificationRepository.GetPagedUserNotificationsAsync(userId, filter, page, pageSize, cancellationToken);
            var dtos = pagedEntities.Items.Adapt<List<NotificationDto>>();

            var response = new PagedResponse<NotificationDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageNumber = pagedEntities.PageNumber,
                PageSize = pagedEntities.PageSize
            };
            return Result<PagedResponse<NotificationDto>>.Success(response);
        }

        // ---------------------- Создание ----------------------
        public async Task<Result<Guid>> CreateNotificationAsync(
            Guid userId,
            CreateNotificationRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = new NotificationEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow
            };

            await _notificationRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = entity.Adapt<NotificationDto>();
            await _hubContext.Clients.Group($"user_{userId}_notifications").SendAsync("NewNotification", dto, cancellationToken);

            return Result<Guid>.Success(entity.Id);
        }

        public async Task<Result<int>> BroadcastNotificationAsync(
            IEnumerable<Guid> userIds,
            CreateNotificationRequest request,
            CancellationToken cancellationToken = default)
        {
            var userIdList = userIds.Distinct().ToList();
            if (!userIdList.Any())
                return Result<int>.Failure("Список получателей пуст");

            var notifications = userIdList.Select(uid => new NotificationEntity
            {
                Id = Guid.NewGuid(),
                UserId = uid,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                IsRead = false,
                CreatedAt = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow
            }).ToList();

            await _notificationRepository.AddRangeAsync(notifications, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var userId in userIdList)
            {
                var dto = notifications.First(n => n.UserId == userId).Adapt<NotificationDto>();
                await _hubContext.Clients.Group($"user_{userId}_notifications").SendAsync("NewNotification", dto, cancellationToken);
            }

            return Result<int>.Success(notifications.Count);
        }

        // ---------------------- Обновление ----------------------
        public async Task<Result<bool>> MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
            if (notification == null || notification.UserId != userId)
                return Result<bool>.Failure("Уведомление не найдено или доступ запрещён");

            notification.IsRead = true;
            notification.LastModified = DateTimeOffset.UtcNow;
            _notificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = notification.Adapt<NotificationDto>();
            await _hubContext.Clients.Group($"user_{userId}_notifications").SendAsync("NotificationUpdated", dto, cancellationToken);

            return Result<bool>.Success(true);
        }

        public async Task<Result<int>> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var updatedCount = await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
            if (updatedCount > 0)
            {
                await _hubContext.Clients.Group($"user_{userId}_notifications").SendAsync("AllNotificationsRead", cancellationToken);
            }
            return Result<int>.Success(updatedCount);
        }

        // ---------------------- Удаление ----------------------
        public async Task<Result<bool>> DeleteNotificationAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
            if (notification == null || notification.UserId != userId)
                return Result<bool>.Failure("Уведомление не найдено или доступ запрещён");

            _notificationRepository.Delete(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _hubContext.Clients.Group($"user_{userId}_notifications").SendAsync("NotificationDeleted", notificationId, cancellationToken);

            return Result<bool>.Success(true);
        }

        public async Task<Result<int>> DeleteAllUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var deletedCount = await _notificationRepository.DeleteAllForUserAsync(userId, cancellationToken);
            if (deletedCount > 0)
            {
                await _hubContext.Clients.Group($"user_{userId}_notifications").SendAsync("AllNotificationsDeleted", cancellationToken);
            }
            return Result<int>.Success(deletedCount);
        }
    }
}