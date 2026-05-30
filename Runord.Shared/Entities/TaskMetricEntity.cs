using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    public class TaskMetricEntity : BaseEntity
    {
        public string TargetClusterNode { get; set; } = string.Empty;
        public double AvgCpuLoad { get; set; }
        public double ComputationImbalance { get; set; }
        public double RamUsageGb { get; set; }
        public double MpiIoPercent { get; set; }
        public int ProcessCount { get; set; }
        public double IoDurationSeconds { get; set; }
        public double AccelerationEfficiency { get; set; }
    }
}
