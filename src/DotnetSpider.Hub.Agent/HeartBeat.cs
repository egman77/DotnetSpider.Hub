using System;
using DotnetSpider.Hub.Agent.Process;

namespace DotnetSpider.Hub.Agent
{
	public class HeartBeat
	{
		public string Ip { get; set; }
		public int CpuLoad { get; set; }
		public long FreeMemory { get; set; }
		public long TotalMemory { get; set; }
		public int ProcessCount { get; set; }
		public int Type { get; set; }
		public string Os { get; set; }
		public string Version { get; set; }
		public int CpuCoreCount { get; set; }
		public string NodeId { get; set; }

		public static HeartBeat Create()
		{
			var heartBeat = new HeartBeat
			{
				NodeId = Env.NodeId,
				Ip = Env.Ip,
				CpuLoad = Convert.ToInt32(Util.GetCpuLoad()),
				Os = Env.Os,
				Version = Env.Version,
				CpuCoreCount = Environment.ProcessorCount,
				Type = Env.NodeType,
				ProcessCount = ProcessManager.ProcessCount,
				FreeMemory = Util.GetFreeMemory(),
				TotalMemory = Util.TotalMemory
			};
			return heartBeat;
		}

		public override string ToString()
		{
			return $"{NodeId}, {CpuLoad}, {FreeMemory}, {TotalMemory}, {ProcessCount}, {CpuCoreCount}";
		}
	}
}
