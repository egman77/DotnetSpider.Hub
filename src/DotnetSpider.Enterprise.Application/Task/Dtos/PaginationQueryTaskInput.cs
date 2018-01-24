using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class PaginationQueryTaskInput : PaginationQueryInput
	{
		public string Keyword { get; set; }
	}
}
