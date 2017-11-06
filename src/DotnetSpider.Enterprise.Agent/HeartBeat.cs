using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Agent
{
    public class HeartBeat
    {
        public long Id { get; set; }
        public string AgentId { get; set; }
        public string Ip { get; set; }
        public int CpuLoad { get; set; }
        public long FreeMemory { get; set; }
        public long TotalMemory { get; set; }
        public long Timestamp { get; set; }
        public int CountOfRunningTasks { get; set; }
        public bool IsEnabled { get; set; }
        public string Os { get; set; }
        public string Version { get; set; } = "0.9.9";
    }
}
