using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
	public class NodeOutputDto
	{
		public virtual string NodeId { get; set; }

		public virtual bool IsEnable { get; set; }

		public virtual bool IsOnline { get; set; }

		public virtual DateTime? LastModificationTime { get; set; }

		public virtual DateTime? CreationTime { get; set; }
	}
}
