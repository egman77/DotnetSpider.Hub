using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Task : AuditedEntity<long>
	{
		[Required]
		[StringLength(100)]
		public virtual string Name { get; set; }

		[StringLength(200)]
		public virtual string Arguments { get; set; }

		[StringLength(100)]
		public virtual string SpiderName { get; set; }

		[Required]
		public virtual int CountOfNodes { get; set; }

		[StringLength(50)]
		public virtual string Cron { get; set; }

		public virtual bool IsEnabled { get; set; }

		[StringLength(100)]
		public virtual string Version { get; set; }

		public virtual int ProjectId { get; set; }

		[ForeignKey("ProjectId")]
		public virtual Project Project { get; set; }
	}
}
