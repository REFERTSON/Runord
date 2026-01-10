using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runord.Client.Shared.ValueObjects
{
    public class NodeMonitoring
    {
        public double CpuUsage { get; set; }
        public double RamUsage { get; set; }
        public int PingMs { get; set; }

        public NodeMonitoring(double cpuUsage, double ramUsage, int pingMs)
        {
            CpuUsage = cpuUsage;
            RamUsage = ramUsage;
            PingMs = pingMs;
        }
    }
}
