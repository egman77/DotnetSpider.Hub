using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Enterprise.Web.Controllers
{
	
	public class HomeController : AppControllerBase
	{
		public HomeController()
		{
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}
	}
}
