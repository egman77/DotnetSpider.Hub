using DotnetSpider.Hub.Application.Report.Dtos;

namespace DotnetSpider.Hub.Application.Report
{
	public interface IDashboardAppService
	{
		HomePageDashboardDto QueryHomeDashboard();
	}
}
