using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.TaskStatus.Dtos
{
	public class PagingQueryTaskStatusInputDto : PagingQueryInputDto
	{
		public string Status { get; set; }
		public string Keyword { get; set; }
	}
}
