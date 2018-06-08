using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers
{
	public class TaskController : BaseController
	{
		public TaskController(ICommonConfiguration commonConfiguration)
			: base(commonConfiguration)
		{
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Create()
		{
			ViewBag.NodeTypes = Configuration.NodeTypes;
			return View();
		}

		public IActionResult Edit()
		{
			ViewBag.NodeTypes = Configuration.NodeTypes;
			return View();
		}

		public IActionResult RunHistory()
		{
			return View();
		}
	}
}
