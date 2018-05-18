using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers
{
	public class HomeController : BaseController
	{
		public HomeController(IAppSession appSession, ICommonConfiguration commonConfiguration)
			: base(appSession, commonConfiguration)
		{
		}

		[HttpGet]
		public IActionResult Index()
		{
			Logger.Information("hello");
			return View();
		}
	}
}
