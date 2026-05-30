using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    public class ClusterMetricEntity : BaseEntity
    {
        public Guid ClusterId { get; set; }
        public DateTime Timestamp { get; set; }
        public double CpuUsagePercent { get; set; }
        public double RamUsageGb { get; set; }
        public double StorageUsageGb { get; set; }
        public double NetworkInMbps { get; set; }
        public double NetworkOutMbps { get; set; }

        public virtual ClusterEntity Cluster { get; set; } = null!;
    }
}