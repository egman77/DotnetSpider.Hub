using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class TaskRunning : AuditedEntity<long>
	{
		/// <summary>
		/// 任务名称
		/// </summary>
		[Required]
		public virtual long TaskId { get; set; }

		[Required]
		[StringLength(32)]
		public virtual string Identity { get; set; }
	}
}
