using Runord.Shared.Base;

namespace Runord.Shared.Entities
{
    // Сущность для представления метрик задач.
    public class TaskMetricEntity : BaseEntity
    {
        // Идентификатор задачи, к которой относятся метрики.
        public Guid TaskId { get; set; }
        public virtual TaskEntity Task { get; set; } = null!;

        // Идентификатор узла кластера, на котором выполнялась задача.
        public string TargetClusterNode { get; set; } = string.Empty;

        // Метрики производительности и использования ресурсов.
        public double AvgCpuLoad { get; set; }             // Средняя загрузка CPU в процентах.
        public double ComputationImbalance { get; set; }   // Метрика, показывающая дисбаланс в вычислениях между разными узлами.
        public double RamUsageGb { get; set; }             // Использование оперативной памяти в гигабайтах.
        public double MpiIoPercent { get; set; }           // Процент времени, затраченного на MPI I/O операции.
        public int ProcessCount { get; set; }              // Количество процессов, участвующих в выполнении задачи.
        public double IoDurationSeconds { get; set; }      // Время, затраченное на I/O операции, в секундах.
        public double AccelerationEfficiency { get; set; } // Эффективность использования ускорителей (например, GPU) в процентах.
    }
}
