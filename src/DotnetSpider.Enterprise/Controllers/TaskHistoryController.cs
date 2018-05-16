using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Enterprise.Controllers
{
	public class TaskHistoryController : AppControllerBase
	{
		public TaskHistoryController(IAppSession appSession, ICommonConfiguration commonConfiguration)
			: base(appSession, commonConfiguration)
		{
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
	}
}
