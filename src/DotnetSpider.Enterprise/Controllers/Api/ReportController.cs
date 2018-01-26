using DotnetSpider.Enterprise.Application.Report;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class ReportController : AppControllerBase
	{
		private readonly IReportAppService _reportAppService;

		public ReportController(IReportAppService reportAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration) : base(appSession, loggerFactory, commonConfiguration)
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
