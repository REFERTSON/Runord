using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Runord.Hub.Hubs;
using Runord.Hub.Services.Interfaces;
using Runord.Shared.DTOs.Task;
using Runord.Shared.Enums;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Grpc
{
    public class ClusterCallbackService : ClusterCallback.ClusterCallbackBase
    {
        private readonly ITaskService _taskService;
        private readonly IFileStorageService _fileStorage;
        private readonly IHubContext<TaskHub> _taskHubContext;
        private readonly ILogger<ClusterCallbackService> _logger;

        public ClusterCallbackService(
            ITaskService taskService,
            IFileStorageService fileStorage,
            IHubContext<TaskHub> taskHubContext,
            ILogger<ClusterCallbackService> logger)
        {
            _taskService = taskService;
            _fileStorage = fileStorage;
            _taskHubContext = taskHubContext;
            _logger = logger;
        }

        public override async Task<MetricsResponse> ReportMetrics(MetricsRequest request, ServerCallContext context)
        {
            try
            {
                var taskId = Guid.Parse(request.TaskId);
                var metricsDto = new TaskMetricsDto(
                    TaskId: taskId,
                    TargetClusterNode: request.TargetClusterNode,
                    AvgCpuLoad: request.AvgCpuLoad,
                    ComputationImbalance: request.ComputationImbalance,
                    RamUsageGb: request.RamUsageGb,
                    MpiIoPercent: request.MpiIoPercent,
                    ProcessCount: request.ProcessCount,
                    IoDurationSeconds: request.IoDurationSeconds,
                    AccelerationEfficiency: request.AccelerationEfficiency,
                    CreatedAt: DateTimeOffset.UtcNow,
                    LastModified: DateTimeOffset.UtcNow
                );
                await _taskService.AddTaskMetricsAsync(taskId, metricsDto, context.CancellationToken);
                return new MetricsResponse { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении метрик задачи {TaskId}", request.TaskId);
                return new MetricsResponse { Success = false };
            }
        }

        public override async Task<ResultResponse> ReportResult(ResultRequest request, ServerCallContext context)
        {
            try
            {
                var taskId = Guid.Parse(request.TaskId);
                var newState = request.IsSuccess ? Shared.Enums.TaskStatus.Completed : Shared.Enums.TaskStatus.Failed;
                await _taskService.UpdateTaskStatusAsync(taskId, newState, null, true, context.CancellationToken);

                if (request.ResultFiles.Any())
                {
                    // Логика сохранения файлов результатов
                }

                await _taskHubContext.Clients.Group($"task_{taskId}").SendAsync("TaskResult", new
                {
                    TaskId = taskId,
                    IsSuccess = request.IsSuccess,
                    ErrorMessage = request.ErrorMessage,
                    ResultFiles = request.ResultFiles
                }, context.CancellationToken);

                return new ResultResponse { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке результата задачи {TaskId}", request.TaskId);
                return new ResultResponse { Success = false };
            }
        }
    }
}