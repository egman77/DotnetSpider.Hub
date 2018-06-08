using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetSpider.Hub.Models;
using Microsoft.AspNetCore.Authorization;
using DotnetSpider.Hub.Core.Configuration;

namespace DotnetSpider.Hub.Controllers
{
	public class HomeController : BaseController
	{
		public HomeController(ICommonConfiguration commonConfiguration)
			: base(commonConfiguration)
		{
		}

		public IActionResult Index()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		[AllowAnonymous]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
