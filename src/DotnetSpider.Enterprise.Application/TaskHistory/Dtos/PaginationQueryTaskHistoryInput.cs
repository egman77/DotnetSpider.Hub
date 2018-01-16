using DotnetSpider.Enterprise.Application.Task.Dtos;
using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Application.TaskHistory.Dtos
{
	public class PaginationQueryTaskHistoryInput : PaginationQueryTaskInput
	{
		[Required]
		public long TaskId { get; set; }
	}
}
