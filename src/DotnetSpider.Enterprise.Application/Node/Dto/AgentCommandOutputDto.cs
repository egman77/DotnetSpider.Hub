using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
	public class AgentCommandOutputDto
	{
		public virtual string AngentId { get; set; }
		public virtual string Task { get; set; }
		public virtual string Name { get; set; }
		public virtual string Arguments { get; set; }
		public virtual string Application { get; set; }
		public virtual string Version { get; set; }
		public virtual string RunId { get; set; }
	}
}
