using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Enums;

namespace Runord.Shared.Interfaces.Services
{
    public interface ITaskService
    {
        Task<Result<List<string>>> GetAvailableTaskTypesAsync(CancellationToken cancellationToken = default);
        Task<Result<TaskDto>> CreateTaskAsync(CreateTaskRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task<Result<TaskDto>> GetTaskByIdAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<Result<PagedResponse<TaskDto>>> GetTasksAsync(Guid? userId, bool isAdmin, Guid? projectId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Result<bool>> UpdateTaskStatusAsync(Guid taskId, Enums.TaskStatus status, Guid? userId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteTaskAsync(Guid id, Guid userId, bool isAdmin, CancellationToken cancellationToken = default);
    }
}