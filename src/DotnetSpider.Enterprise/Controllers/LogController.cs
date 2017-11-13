using DotnetSpider.Enterprise.Application.Log;
using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Controllers
{
	public class LogController : AppControllerBase
	{
		private readonly ILogAppService _logAppService;

		public LogController(IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration,
			ILogAppService logAppService)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_logAppService = logAppService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		[AllowAnonymous]
		public IActionResult Submit([FromBody] LogInputDto input)
		{
			_logAppService.Sumit(input);
			return Ok();
		}

		[HttpPost]
		public IActionResult QueryLogs(PagingLogInputDto input)
		{
			return ActionResult(_logAppService.QueryLogs, input);
		}
	}
}
