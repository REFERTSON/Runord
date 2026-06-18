using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    // Сущность, представляющая метрику кластера в системе.
    public class ClusterMetricEntity : BaseEntity
    {
        // Идентификатор кластера, к которому относится эта метрика.
        public Guid ClusterId { get; set; }
        public virtual ClusterEntity Cluster { get; set; } = null!;

        // Временная метка, когда была собрана эта метрика.
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Процент использования CPU в кластере.
        public double CpuUsagePercent { get; set; } = 0.0;

        // Использование оперативной памяти в гигабайтах.
        public double RamUsageGb { get; set; } = 0.0;

        // Количество задач, выполняющихся в кластере.
        public int CountTasks { get; set; }
    }
}