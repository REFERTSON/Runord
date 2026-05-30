using Runord.Shared.Base;
using Runord.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Runord.Shared.DTOs.Cluster
{
    public record ClusterDto(
        Guid Id,
        string Name,
        string IpAddress,
        ClusterStatus Status,
        double CpuTotalPercent,
        double RamTotalGb
    ) : BaseDto(Id);

    public record ShortClusterMetricDto(
        double CpuUsagePercent,
        double RamUsageGb
    );

    public record ClusterMetricsDto(
        Guid NodeId,
        ClusterStatus Status,
        double CpuUsagePercent,
        double CpuTotalPercent,
        double RamUsageGb,
        double StorageUsageGb,
        double NetworkInMbps,
        double NetworkOutMbps
    );

    public record CreateClusterRequest(string Name, string IpAddress, double RamTotalGb, double StorageTotalGb);
    public record UpdateClusterRequest(Guid Id, ClusterDto Cluster);
    public record DeleteClusterRequest(Guid Id);
}
