using DotnetSpider.Hub.Application.Report;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class DashboardController : BaseController
	{
		private readonly IDashboardAppService _dashboardAppService;

		public DashboardController(IDashboardAppService reportAppService, ICommonConfiguration commonConfiguration) : base(commonConfiguration)
		{
			_dashboardAppService = reportAppService;
		}

		[HttpGet]
		public IActionResult QueryHomeDashboard()
		{
			return Success(_dashboardAppService.QueryHomeDashboard());
		}
	}
}
