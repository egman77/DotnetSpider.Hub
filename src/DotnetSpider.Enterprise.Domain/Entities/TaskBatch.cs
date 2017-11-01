using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class TaskBatch : Entity<long>
	{
		public virtual long TaskId { get; set; }
		public virtual string Name { get; set; }
		public virtual DateTime Batch { get; set; }
		public virtual string Identity { get; set; }
	}
}
