using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class TaskLog : Entity<long>
	{
		[Required]
		[StringLength(32)]
		public string Identity { get; set; }

		[Required]
		[StringLength(32)]
		public string NodeId { get; set; }

		public DateTime? Logged { get; set; }

		public string Level { get; set; }

		public string Message { get; set; }

		public string Exception { get; set; }
	}
}
