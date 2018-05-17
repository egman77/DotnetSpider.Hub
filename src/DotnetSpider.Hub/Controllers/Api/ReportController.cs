using DotnetSpider.Hub.Application.Report;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class ReportController : BaseController
	{
		private readonly IReportAppService _reportAppService;

		public ReportController(IReportAppService reportAppService, IAppSession appSession, ICommonConfiguration commonConfiguration) : base(appSession, commonConfiguration)
		{
			_reportAppService = reportAppService;
		}

		[HttpGet]
		public IActionResult Find(FilterQueryInput input)
		{
			return Success(_reportAppService.Query(input));
		}
	}
}
