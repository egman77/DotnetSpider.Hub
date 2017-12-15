using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Application.TaskHistory.Dtos
{
	public class AddTaskHistoryInputDto
	{
		[Required]
		public virtual long TaskId { get; set; }

		[Required]
		[StringLength(32)]
		public virtual string Identity { get; set; }

		public virtual string NodeIds { get; set; }
	}
}
