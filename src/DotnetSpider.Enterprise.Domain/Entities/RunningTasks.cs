using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class RunningTasks
	{
		public virtual long Id { get; set; }
		public virtual DateTime CDate { get; set; }
		public virtual long TaskId { get; set; }

		public virtual string Identity { get; set; }
	}
}
