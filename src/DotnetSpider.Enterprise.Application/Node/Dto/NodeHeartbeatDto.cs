namespace DotnetSpider.Enterprise.Application.Node.Dto
{
	public class NodeHeartbeatDto
	{
		public virtual string NodeId { get; set; }
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
