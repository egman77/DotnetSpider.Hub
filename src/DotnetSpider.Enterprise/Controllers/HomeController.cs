using DotnetSpider.Enterprise.Application.Report;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DotnetSpider.Enterprise.Web.Controllers
{
	public class HomeController : AppControllerBase
	{
		private readonly IReportAppService _reportAppService;

		public HomeController(IReportAppService reportAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_reportAppService = reportAppService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Dashboard()
		{
			return DataResult(_reportAppService.GetHomePageDashboard());
		}

		[HttpGet]
		public IActionResult ThrowException()
		{
			throw new Exception("TEST EXCEPTION.");
		}
	}
}
