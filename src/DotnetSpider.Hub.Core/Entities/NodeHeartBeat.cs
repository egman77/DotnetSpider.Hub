using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Core.Entities
{
	public class NodeHeartbeat : AuditedEntity
	{
		[Required]
		[StringLength(32)]
		public virtual string NodeId { get; set; }
		[StringLength(20)]
		public virtual string Ip { get; set; }
		public virtual int CpuLoad { get; set; }
		public virtual int CpuCoreCount { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual int ProcessCount { get; set; }
		[StringLength(100)]
		public virtual string Os { get; set; }
		/// <summary>
		/// 1: 内部节点  2: VPS节点
		/// </summary>
		[Required]
		public virtual string Type { get; set; }
		[StringLength(50)]
		public virtual string Version { get; set; }
	}
}
