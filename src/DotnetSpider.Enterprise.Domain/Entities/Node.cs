using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Node : AuditedEntity<long>
	{
		[Required]
		[StringLength(32)]
		public virtual string NodeId { get; set; }

		public virtual bool IsEnable { get; set; }

		public virtual bool IsOnline { get; set; }

		/// <summary>
		/// 1: 内部节点  2: VPS节点
		/// </summary>
		[Required]
		public virtual int Type { get; set; }

		public virtual string Os { get; set; }

		public override int GetHashCode()
		{
			return NodeId.GetHashCode();
		}
	}
}
