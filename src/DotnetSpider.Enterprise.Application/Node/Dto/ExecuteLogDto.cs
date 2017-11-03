using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
	public class ExecuteLogDto
	{
		public virtual long Id { get; set; }

		public virtual string TaskId { get; set; }

		public virtual string CommandId { get; set; }

		public virtual string NodeId { get; set; }

		public virtual string NodeIp { get; set; }

		public virtual string LogType { get; set; }

		public virtual string Message { get; set; }
		public virtual DateTime CreationTime { get; set; }
	}
}
