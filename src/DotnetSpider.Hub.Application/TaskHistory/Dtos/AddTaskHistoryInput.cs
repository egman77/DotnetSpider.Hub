using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Application.TaskHistory.Dtos
{
	public class AddTaskHistoryInput
	{
		[Required]
		public virtual string TaskId { get; set; }

		[Required]
		[StringLength(32)]
		public virtual string Identity { get; set; }

		public virtual string NodeIds { get; set; }
	}
}
