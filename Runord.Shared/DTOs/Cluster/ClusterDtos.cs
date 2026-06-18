using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.DTOs.Cluster
{
    public record ClusterDto(
        Guid Id,
        string Name,
        string IpAddress,
        ClusterStatus Status,
        double CpuTotalPercent,
        double RamTotalGb,
        double CpuUsagePercent,
        double RamUsageGb
    ) : BaseDto(Id);

    public record CreateClusterRequest(string Name, string IpAddress);
    public record UpdateClusterRequest(Guid Id, string Name, string IpAddress);
    public record ClusterChangedNotification(
        ClusterAction Action,
        ClusterDto Cluster
    );
}