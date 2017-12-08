using DotnetSpider.Enterprise.Application.Report.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Report
{
	public interface IReportAppService
	{
		HomePageDashboardOutputDto GetHomePageDashboard();
	}
}
