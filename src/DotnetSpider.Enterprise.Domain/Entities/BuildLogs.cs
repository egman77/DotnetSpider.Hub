using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class BuildLog : Entity<long>
	{
		public virtual long BuildId { get; set; }
		[StringLength(32)]
		public virtual string LogType { get; set; }
		[StringLength(2000)]
		public virtual string Message { get; set; }
		public virtual DateTime CreationTime { get; set; }
	}
}
