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
		public virtual string Ip { get; set; }
		public virtual int CPULoad { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual int ProcessCount { get; set; }
		public virtual string Os { get; set; }
		public virtual string Version { get; set; }
	}
}
