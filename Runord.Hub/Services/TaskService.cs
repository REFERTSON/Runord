using Mapster;
using Runord.Hub.Data.Repositories.Interfaces;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Entities;
using Runord.Shared.Enums;
using Runord.Shared.Interfaces;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IQueueService _queueService;
        private readonly IClusterSelector _clusterSelector;

        public TaskService(
            ITaskRepository taskRepository,
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IQueueService queueService,
            IClusterSelector clusterSelector)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _queueService = queueService;
            _clusterSelector = clusterSelector;
        }

        public async Task<Result<List<string>>> GetAvailableTaskTypesAsync(CancellationToken cancellationToken = default)
        {
            // В реальности можно брать из БД или enum
            var types = new List<string> { "Матричное умножение", "Вычисление Pi", "Обработка изображений" };
            return Result<List<string>>.Success(types);
        }

        public async Task<Result<TaskDto>> CreateTaskAsync(CreateTaskRequest request, Guid userId, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
            if (project == null)
                return Result<TaskDto>.Failure("Проект не найден.");

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
                Files = request.InputFiles.Select(f => new TaskFile
                {
                    Id = Guid.NewGuid(),
                    Name = f,
                    IsResult = false,
                    CreatedAt = DateTimeOffset.UtcNow
                }).ToList()
            };

            await _taskRepository.AddAsync(taskEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var queueMessage = new TaskQueueMessage
            {
                TaskId = taskEntity.Id,
                TaskType = taskEntity.TaskType,
                Name = taskEntity.Name,
                Priority = (int)taskEntity.Priority,
                InputFilePaths = request.InputFiles,
                OwnerId = userId,
                CreatedAt = taskEntity.CreatedAt
            };
            await _queueService.PublishTaskAsync(queueMessage, cancellationToken);

            var dto = taskEntity.Adapt<TaskDto>();
            return Result<TaskDto>.Success(dto);
        }

        public async Task<Result<TaskDto>> GetTaskByIdAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetTaskWithDetailsAsync(id, cancellationToken);
            if (task == null)
                return Result<TaskDto>.Failure("Задача не найдена.");

            if (!isAdmin && task.OwnerId != userId)
                return Result<TaskDto>.Failure("Нет прав.");

            var dto = task.Adapt<TaskDto>();
            return Result<TaskDto>.Success(dto);
        }

        public async Task<Result<PagedResponse<TaskDto>>> GetTasksAsync(Guid? userId, bool isAdmin, Guid? projectId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _taskRepository.GetFilteredTasksAsync(userId, isAdmin, projectId, page, pageSize, cancellationToken);
            var dtos = items.Adapt<List<TaskDto>>();
            var response = new PagedResponse<TaskDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
            return Result<PagedResponse<TaskDto>>.Success(response);
        }

        public async Task<Result<bool>> UpdateTaskStatusAsync(Guid taskId, Shared.Enums.TaskStatus status, Guid? userId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
            if (task == null)
                return Result<bool>.Failure("Задача не найдена.");

            if (!isAdmin && userId.HasValue && task.OwnerId != userId.Value)
                return Result<bool>.Failure("Нет прав.");

            task.Status = status;
            task.LastModified = DateTimeOffset.UtcNow;
            _taskRepository.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteTaskAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
            if (task == null)
                return Result<bool>.Failure("Задача не найдена.");

            if (!isAdmin && task.OwnerId != userId)
                return Result<bool>.Failure("Нет прав.");

            _taskRepository.Delete(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}