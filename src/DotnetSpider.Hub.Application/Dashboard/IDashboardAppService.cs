using DotnetSpider.Hub.Application.Report.Dtos;
using DotnetSpider.Hub.Core.Entities;

namespace DotnetSpider.Hub.Application.Report
{
	public interface IDashboardAppService
	{
		HomePageDashboardDto QueryHomeDashboard();
	}
}
