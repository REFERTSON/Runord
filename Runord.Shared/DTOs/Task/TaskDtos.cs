using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.Task
{
    // DTO для передачи информации о задаче.
    public record TaskDto(
        Guid Id,
        Guid ProjectId,
        string Name,
        string Type,
        string OwnerName,
        Enums.TaskStatus Status,
        TaskPriority Priority,
        int ProgressPercent,
        DateTimeOffset CreatedAt,
        List<TaskFileDto> Files
    ) : BaseDto(Id);

    // DTO для метрик задачи (плоская структура, без вложенного Task)
    public record TaskMetricsDto(
        Guid TaskId,
        string TargetClusterNode,
        double AvgCpuLoad,
        double ComputationImbalance,
        double RamUsageGb,
        double MpiIoPercent,
        int ProcessCount,
        double IoDurationSeconds,
        double AccelerationEfficiency,
        DateTimeOffset CreatedAt,
        DateTimeOffset LastModified
    );

    // DTO для создания задачи.
    public record CreateTaskRequest(
        Guid ProjectId,
        string Name,
        string Type,
        TaskPriority Priority,
        List<string> InputFiles
    );

    public record UpdateTaskStatusRequest(Enums.TaskStatus Status);
}