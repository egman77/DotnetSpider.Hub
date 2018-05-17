using System;
using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Core.Entities
{
	public class TaskLog : Entity
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
