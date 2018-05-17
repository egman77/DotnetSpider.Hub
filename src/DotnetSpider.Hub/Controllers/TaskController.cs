using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers
{
	public class TaskController : BaseController
	{
		public TaskController(IAppSession appSession, ICommonConfiguration commonConfiguration)
			: base(appSession, commonConfiguration)
		{
		}

		[HttpGet]
		public IActionResult Index()
		{
			ViewBag.AgentTypes = Configuration.AgentTypes;
			return View();
		}

		[HttpGet]
		public IActionResult RunHistory()
		{
			return View();
		}
	}
}
