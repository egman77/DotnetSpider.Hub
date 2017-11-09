using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
	public class NodeHeartbeatInputDto
	{
		public virtual string AgentId { get; set; }
		public virtual string Ip { get; set; }
		public virtual int CpuLoad { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual long Timestamp { get; set; }
		public virtual int CountOfRunningTasks { get; set; }
		public virtual string Os { get; set; }
		public virtual string Version { get; set; }
	}
}
