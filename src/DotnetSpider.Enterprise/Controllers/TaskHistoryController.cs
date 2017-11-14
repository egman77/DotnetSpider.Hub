using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Controllers
{
	public class TaskHistoryController : AppControllerBase
	{
		public readonly ITaskHistoryAppService _taskHistoryAppService;

		public TaskHistoryController(ITaskHistoryAppService taskHistoryAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_taskHistoryAppService = taskHistoryAppService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Query(PagingQueryTaskHistoryInputDto input)
		{
			input.Sort = "desc";
			return DataResult(_taskHistoryAppService.Query(input));
		}
	}
}
