using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
{
	public class TaskHistoryController : AppControllerBase
	{
		public TaskHistoryController(IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
	}
}
