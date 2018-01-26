using DotnetSpider.Enterprise.Application.Report.Dtos;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Report
{
	public interface IReportAppService
	{
		dynamic Query(FilterQueryInput input);
	}
}
