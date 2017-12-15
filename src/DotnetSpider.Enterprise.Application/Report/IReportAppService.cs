using DotnetSpider.Enterprise.Application.Report.Dtos;

namespace DotnetSpider.Enterprise.Application.Report
{
	public interface IReportAppService
	{
		HomePageDashboardOutputDto GetHomePageDashboard();
	}
}
