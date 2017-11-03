using DotnetSpider.Enterprise.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class AgentHeartBeat : AuditedEntity<long>
	{
		[Required]
		[StringLength(20)]
		public virtual string AgentId { get; set; }
		[StringLength(20)]
		public virtual string Ip { get; set; }
		public virtual int CpuLoad { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual long Timestamp { get; set; }
		public virtual int CountOfRunningTasks { get; set; }
		public virtual bool IsEnabled { get; set; }
		[StringLength(20)]
		public virtual string Os { get; set; }
		[StringLength(5)]
		public virtual string Version { get; set; } = "0.9.9";
	}
}
