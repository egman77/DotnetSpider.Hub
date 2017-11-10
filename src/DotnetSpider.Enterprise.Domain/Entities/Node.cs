using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Node : AuditedEntity<long>
	{
		[Required]
		[StringLength(20)]
		public virtual string NodeId { get; set; }

		public virtual bool IsEnable { get; set; }

		public virtual bool IsOnline { get; set; }
	}
}
