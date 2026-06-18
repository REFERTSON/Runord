using Runord.Shared.Base;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Enums;
using Runord.Shared.Filters;

namespace Runord.Shared.Interfaces.Services
{
    public interface ITaskService
    {
        Task<Response<IEnumerable<string>>> GetAvailableTaskTypesAsync(CancellationToken ct = default);
        Task<Response<IEnumerable<TaskDto>>> GetTasksAsync(TaskFilter filter, Guid userId, bool isAdmin, CancellationToken ct = default);
        Task<Response<TaskDto>> GetTaskByIdAsync(Guid id, Guid userId, bool isAdmin, CancellationToken ct = default);
        Task<Response<TaskDto>> CreateTaskAsync(CreateTaskRequest request, Guid userId, CancellationToken ct = default);
        Task<Response<bool>> UpdateTaskStatusAsync(Guid taskId, Enums.TaskStatus status, Guid? userId, bool isAdmin, CancellationToken ct = default);
        Task<Response<bool>> DeleteTaskAsync(Guid id, Guid userId, bool isAdmin, CancellationToken ct = default);
        // Доп. метод для добавления метрик задачи (вызывается из gRPC)
        Task<Response<bool>> AddTaskMetricsAsync(Guid taskId, TaskMetricsDto metrics, CancellationToken ct = default);
    }
}