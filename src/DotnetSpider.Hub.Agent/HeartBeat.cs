using System;
using DotnetSpider.Hub.Agent.Process;

namespace DotnetSpider.Hub.Agent
{
	public class HeartBeat
	{
        /// <summary>
        /// ip地址
        /// </summary>
		public string Ip { get; set; }
        /// <summary>
        /// CPU负载量
        /// </summary>
		public int CpuLoad { get; set; }
        /// <summary>
        /// 空闲内存
        /// </summary>
		public long FreeMemory { get; set; }
        /// <summary>
        /// 总计内存
        /// </summary>
		public long TotalMemory { get; set; }
        /// <summary>
        /// 线程数
        /// </summary>
		public int ProcessCount { get; set; }
        /// <summary>
        /// 处理器类型
        /// </summary>
		public string Type { get; set; }
        /// <summary>
        /// 操作系统
        /// </summary>
		public string Os { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
		public string Version { get; set; }
        /// <summary>
        /// 处理器核心数
        /// </summary>
		public int CpuCoreCount { get; set; }
        /// <summary>
        /// 节点ID
        /// </summary>
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
