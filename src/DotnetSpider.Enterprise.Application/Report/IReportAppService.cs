using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.Application.Report
{
	public interface IReportAppService
	{
		dynamic Query(FilterQueryInput input);
	}
}
