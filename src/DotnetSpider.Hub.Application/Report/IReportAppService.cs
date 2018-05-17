using DotnetSpider.Hub.Core.Entities;

namespace DotnetSpider.Hub.Application.Report
{
	public interface IReportAppService
	{
		dynamic Query(FilterQueryInput input);
	}
}
