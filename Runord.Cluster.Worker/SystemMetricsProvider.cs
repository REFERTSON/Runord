using Hardware.Info;

namespace Runord.Cluster.Worker.Services;

public class SystemMetricsProvider
{
    private readonly IHardwareInfo _hardwareInfo = new HardwareInfo();

    public (double cpuLoad, double ramUsagePercent) GetMetrics()
    {
        // Refresh обновляет данные из системных API
        _hardwareInfo.RefreshCPUList();
        _hardwareInfo.RefreshMemoryList();

        // Считаем среднюю нагрузку по всем ядрам
        double cpuLoad = _hardwareInfo.CpuList.Average(c => c.PercentProcessorTime);

        // Считаем процент использования RAM
        var memory = _hardwareInfo.MemoryList.FirstOrDefault();
        double ramUsagePercent = memory != null
            ? (double)memory.UsedPhysicalMemory / memory.Capacity * 100
            : 0;

        return (cpuLoad, ramUsagePercent);
    }
}