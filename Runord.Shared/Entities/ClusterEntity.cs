using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    public class ClusterEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public ClusterStatus Status { get; set; }
        public double CpuTotal { get; set; }
        public double RamTotalGb { get; set; }
        public double StorageTotalGb { get; set; }

        // Метрики кластера (текущее состояние)
        public double CpuUsagePercent { get; set; }
        public double RamUsageGb { get; set; }
        public double StorageUsageGb { get; set; }
        public double NetworkInMbps { get; set; }
        public double NetworkOutMbps { get; set; }
    }
}