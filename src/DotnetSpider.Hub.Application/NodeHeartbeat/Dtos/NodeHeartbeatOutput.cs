using System;

namespace DotnetSpider.Hub.Application.NodeHeartbeat.Dtos
{
	public class NodeHeartbeatOutput
	{
		public virtual string NodeId { get; set; }
		public virtual string Task { get; set; }
		public virtual string Name { get; set; }
		public virtual string Arguments { get; set; }
		public virtual string ApplicationName { get; set; }
		public virtual string Package { get; set; }
		public virtual string TaskId { get; set; }
		public virtual DateTime CreationTime { get; set; }
	}
}
