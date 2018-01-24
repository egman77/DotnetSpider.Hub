using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class TaskIdInput
	{
		[Required]
		public virtual long TaskId { get; set; }
	}
}
