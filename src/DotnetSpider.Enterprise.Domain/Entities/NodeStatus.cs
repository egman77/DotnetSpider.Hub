using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class NodeStatus
	{
		public virtual string Identity { get; set; }
		public virtual DateTime Logged { get; set; }
		public virtual string Status { get; set; }
		public virtual int Thread { get; set; }
		public virtual long Left { get; set; }
		public virtual long Success { get; set; }
		public virtual long Error { get; set; }
		public virtual long Total { get; set; }
		public virtual long AvgDownloadSpeed { get; set; }
		public virtual long AvgProcessorSpeed { get; set; }
		public virtual string Node { get; set; }
		public virtual long AvgPipelineSpeed { get; set; }
	}
}
