using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class TaskStatus : AuditedEntity<long>
	{
		[Required]
		public virtual long TaskId { get; set; }
		[Required]
		[StringLength(32)]
		public virtual string Identity { get; set; }
		[Required]
		[StringLength(32)]
		public virtual string NodeId { get; set; }
		[StringLength(20)]
		public virtual string Status { get; set; }
		public virtual int Thread { get; set; }
		public virtual long Left { get; set; }
		public virtual long Success { get; set; }
		public virtual long Error { get; set; }
		public virtual long Total { get; set; }
		public virtual float AvgDownloadSpeed { get; set; }
		public virtual float AvgProcessorSpeed { get; set; }
		public virtual float AvgPipelineSpeed { get; set; }
	}
}
