using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Message : AuditedEntity<long>
	{
		[Required]
		[StringLength(32)]
		public virtual string NodeId { get; set; }

		[StringLength(100)]
		public virtual string Name { get; set; }

		[StringLength(100)]
		[Required]
		public virtual string ApplicationName { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		[StringLength(500)]
		public virtual string Arguments { get; set; }
	}
}
