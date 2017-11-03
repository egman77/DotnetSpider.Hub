using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
    public class NodeInfoDto
    {
		public string AgentId { get; set; }
		public string Ip { get; set; }
		public string CpuLoad { get; set; }
		public string FreeMemory { get; set; }
		public string TotalMemory { get; set; }
		public string Timestamp { get; set; }
		public string CountOfRunningTasks { get; set; }
		public string IsOnline { get; set; }
		public string IsEnabled { get; set; }
		public string Os { get; set; }
		public string Version { get; set; }
	}

	public class NodeStatusDto
	{
		public virtual string Identity { get; set; }
		public virtual DateTime Logged { get; set; }
		public virtual string Status { get; set; }
		public virtual int Thread { get; set; }
		public virtual long Left { get; set; }
		public virtual long Success { get; set; }
		public virtual long Error { get; set; }
		public virtual long Total { get; set; }
		public virtual long AvgDownloadSpeed { get; set; }
		public virtual long AvgProcessorSpeed { get; set; }
		public virtual string Node { get; set; }
		public virtual long AvgPipelineSpeed { get; set; }
	}
}
