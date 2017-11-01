using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class TaskLog
	{
		public virtual string Identity { get; set; }
		public virtual string Node { get; set; }
		public virtual DateTime Logged { get; set; }
		public virtual string Level { get; set; }
		public virtual string Message { get; set; }
		public virtual string CallSite { get; set; }
		public virtual string Exception { get; set; }
		public virtual long Id { get; set; }
	}
}
