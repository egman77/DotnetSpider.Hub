using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers
{
	public class TaskStatusController : BaseController
	{
		public TaskStatusController(ICommonConfiguration commonConfiguration)
			: base(commonConfiguration)
		{
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
