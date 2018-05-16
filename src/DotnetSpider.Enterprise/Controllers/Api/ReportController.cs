using DotnetSpider.Enterprise.Application.Report;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class ReportController : AppControllerBase
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
