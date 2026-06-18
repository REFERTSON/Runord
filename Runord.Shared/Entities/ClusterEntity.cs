using Runord.Shared.Base;
using Runord.Shared.Enums;

namespace Runord.Shared.Entities
{
    public class ClusterEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public ClusterStatus Status { get; set; } = ClusterStatus.Offline;

        // Общая мощность (максимальные значения)
        public double CpuTotalPercent { get; set; } = 0.0;
        public double RamTotalGb { get; set; } = 0.0;

        // Текущие метрики использования
        public double CpuUsagePercent { get; set; } = 0.0;
        public double RamUsageGb { get; set; } = 0.0;
    }
}