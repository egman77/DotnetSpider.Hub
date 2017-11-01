using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
    public class ExecuteLog:Entity<long>
    {
		[StringLength(50)]
		public virtual string TaskId { get; set; }
		[StringLength(50)]
		public virtual string CommandId { get; set; }
		[StringLength(50)]
		public virtual string NodeId { get; set; }
		[StringLength(50)]
		public virtual string NodeIp { get; set; }
		[StringLength(50)]
		public virtual string LogType { get; set; }
		[StringLength(2000)]
		public virtual string Message { get; set; }
		public virtual DateTime CreationTime { get; set; }
	}
}
