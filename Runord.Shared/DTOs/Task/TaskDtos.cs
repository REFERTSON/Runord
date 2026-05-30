using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.Task
{
    public record TaskDto(
        Guid Id,
        Guid ProjectId,
        string Name,
        string Type,
        string OwnerName,
        Enums.TaskStatus Status,
        TaskPriority Priority,
        int ProgressPercent,
        DateTimeOffset CreatedAt
    ) : BaseDto(Id);

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
        List<TaskFileDto> Files,
        DateTimeOffset CreatedAt,
        DateTimeOffset LastModified
    );

    public record TaskFileDto(
        Guid Id, 
        string Name, 
        long SizeBytes, 
        string Md5Hash, 
        bool IsResult
    ) : BaseDto(Id);

    public record CreateTaskRequest(
        Guid ProjectId,
        string Name,
        string Type,
        TaskPriority Priority,
        List<string> InputFiles
    );
    public record UpdateTaskStatusRequest(Enums.TaskStatus Status);
    public record DeleteTaskRequest(Guid Id);
}
