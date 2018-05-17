using System;

namespace DotnetSpider.Hub.Application.Node.Dtos
{
	public class NodeDto
	{
		public virtual string NodeId { get; set; }
		public virtual bool IsEnable { get; set; }
		public virtual bool IsOnline { get; set; }
		public virtual DateTime? LastModificationTime { get; set; }
		public virtual DateTime? CreationTime { get; set; }
		public virtual string Ip { get; set; }
		public virtual int CPULoad { get; set; }
		public virtual int CPUCoreCount { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual int ProcessCount { get; set; }
		public virtual string Os { get; set; }
		public virtual int Type { get; set; }
		public virtual string Version { get; set; }
	}
}
