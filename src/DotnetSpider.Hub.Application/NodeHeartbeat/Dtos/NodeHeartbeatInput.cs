using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Application.NodeHeartbeat.Dtos
{
	public class NodeHeartbeatInput
	{
		[Required]
		[StringLength(32)]
		public virtual string NodeId { get; set; }
		[StringLength(20)]
		public virtual string Ip { get; set; }
		public virtual int CPULoad { get; set; }
		public virtual int CPUCoreCount { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual int ProcessCount { get; set; }
		[StringLength(100)]
		public virtual string Os { get; set; }
		[Required]
		public virtual string Type { get; set; }
		[StringLength(50)]
		public virtual string Version { get; set; }
	}
}
