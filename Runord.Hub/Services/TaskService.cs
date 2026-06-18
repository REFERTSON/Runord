using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Hubs;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskFileRepository _taskFileRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IQueueService _queueService;
        private readonly IClusterSelector _clusterSelector;
        private readonly IHubContext<TaskHub> _taskHubContext;

        public TaskService(
            ITaskRepository taskRepository,
            ITaskFileRepository taskFileRepository,
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IQueueService queueService,
            IClusterSelector clusterSelector,
            IHubContext<TaskHub> taskHubContext)
        {
            _taskRepository = taskRepository;
            _taskFileRepository = taskFileRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _queueService = queueService;
            _clusterSelector = clusterSelector;
            _taskHubContext = taskHubContext;
        }

        private async Task NotifyTaskChangedAsync(string action, TaskEntity task, CancellationToken ct)
        {
            var dto = task.Adapt<TaskDto>();
            await _taskHubContext.Clients.Group($"task_{task.Id}")
                .SendAsync("TaskChanged", action, dto, ct);
        }

        public async Task<Response<IEnumerable<string>>> GetAvailableTaskTypesAsync(CancellationToken ct = default)
        {
            // Можно загружать из БД или конфига
            var types = new List<string> { "MatrixMultiplication", "PiCalculation", "ImageProcessing" };
            return Response<IEnumerable<string>>.Success(types);
        }

        public async Task<Response<IEnumerable<TaskDto>>> GetTasksAsync(
            TaskFilter filter, Guid userId, bool isAdmin, CancellationToken ct = default)
        {
            var query = _taskRepository.GetQueryable();

            if (!isAdmin)
                query = query.Where(t => t.OwnerId == userId);

            if (filter.OwnerId.HasValue && (isAdmin || filter.OwnerId == userId))
                query = query.Where(t => t.OwnerId == filter.OwnerId.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
                query = query.Where(t => t.Name.Contains(filter.SearchText) || t.TaskType.Contains(filter.SearchText));

            if (filter.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.ToDate.Value);

            var tasks = await query
                .Include(t => t.Files)
                .Include(t => t.Owner)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);

            var dtos = tasks.Adapt<List<TaskDto>>();
            return Response<IEnumerable<TaskDto>>.Success(dtos);
        }

        public async Task<Response<TaskDto>> GetTaskByIdAsync(Guid id, Guid userId, bool isAdmin, CancellationToken ct = default)
        {
            var task = await _taskRepository.GetTaskWithDetailsAsync(id, ct);
            if (task == null)
                return Response<TaskDto>.Failure("Задача не найдена.");

            if (!isAdmin && task.OwnerId != userId)
                return Response<TaskDto>.Failure("Нет прав доступа.");

            var dto = task.Adapt<TaskDto>();
            return Response<TaskDto>.Success(dto);
        }

        public async Task<Response<TaskDto>> CreateTaskAsync(CreateTaskRequest request, Guid userId, CancellationToken ct = default)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, ct);
            if (project == null)
                return Response<TaskDto>.Failure("Проект не найден.");

            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                Name = request.Name,
                TaskType = request.Type,
                Priority = request.Priority,
                Status = Shared.Enums.TaskStatus.Created,
                OwnerId = userId,
                ProgressPercent = 0,
                CreatedAt = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow,
                Files = request.InputFiles.Select(f => new TaskFileEntity
                {
                    Id = Guid.NewGuid(),
                    Name = f,
                    IsResult = false,
                    CreatedAt = DateTimeOffset.UtcNow
                }).ToList()
            };

            await _taskRepository.AddAsync(taskEntity, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Отправляем в очередь
            var queueMessage = new TaskQueueMessage
            {
                TaskId = taskEntity.Id,
                TaskType = taskEntity.TaskType,
                Name = taskEntity.Name,
                Priority = taskEntity.Priority,
                InputFilePaths = request.InputFiles,
                OwnerId = userId,
                CreatedAt = taskEntity.CreatedAt
            };
            await _queueService.PublishTaskAsync(queueMessage, ct);

            await NotifyTaskChangedAsync("Create", taskEntity, ct);
            var dto = taskEntity.Adapt<TaskDto>();
            return Response<TaskDto>.Success(dto);
        }

        public async Task<Response<bool>> UpdateTaskStatusAsync(Guid taskId, Shared.Enums.TaskStatus status, Guid? userId, bool isAdmin, CancellationToken ct = default)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, ct);
            if (task == null)
                return Response<bool>.Failure("Задача не найдена.");

            if (!isAdmin && userId.HasValue && task.OwnerId != userId.Value)
                return Response<bool>.Failure("Нет прав.");

            task.Status = status;
            task.LastModified = DateTimeOffset.UtcNow;
            _taskRepository.Update(task);
            await _unitOfWork.SaveChangesAsync(ct);

            await NotifyTaskChangedAsync("StatusUpdate", task, ct);
            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> DeleteTaskAsync(Guid id, Guid userId, bool isAdmin, CancellationToken ct = default)
        {
            var task = await _taskRepository.GetTaskWithDetailsAsync(id, ct);
            if (task == null)
                return Response<bool>.Failure("Задача не найдена.");

            if (!isAdmin && task.OwnerId != userId)
                return Response<bool>.Failure("Нет прав.");

            // Удаляем файлы из MinIO
            foreach (var file in task.Files)
            {
                await _fileStorage.DeleteFileAsync("runord", file.Name, ct);
            }

            _taskRepository.Delete(task);
            await _unitOfWork.SaveChangesAsync(ct);

            await _taskHubContext.Clients.Group($"task_{id}").SendAsync("TaskDeleted", id, ct);
            return Response<bool>.Success(true);
        }

        public async Task<Response<bool>> AddTaskMetricsAsync(Guid taskId, TaskMetricsDto metrics, CancellationToken ct = default)
        {
            var metricEntity = metrics.Adapt<TaskMetricEntity>();
            metricEntity.TaskId = taskId;
            metricEntity.Id = Guid.NewGuid();
            metricEntity.CreatedAt = DateTimeOffset.UtcNow;
            // Сохранить метрику (нужен репозиторий для метрик, создайте по аналогии)
            // await _taskMetricRepository.AddAsync(metricEntity, ct);
            // await _unitOfWork.SaveChangesAsync(ct);
            return Response<bool>.Success(true);
        }
    }
}