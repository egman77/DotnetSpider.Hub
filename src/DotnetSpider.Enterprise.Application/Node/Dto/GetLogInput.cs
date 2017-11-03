using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
    public class GetLogInput : PagingQueryInputDto
	{
		public string AgentId { get; set; }
		public string LogLevel { get; set; }
	}
}
