using System;
using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Application.TaskLog.Dtos
{
	public class AddTaskLogInput
	{
		[Required]
		public string Identity { get; set; }

		[Required]
		public string NodeId { get; set; }
	
		public DateTime? Logged { get; set; } 

		public string Level { get; set; }
		public string Message { get; set; }
		public string Exception { get; set; }
	}
}
