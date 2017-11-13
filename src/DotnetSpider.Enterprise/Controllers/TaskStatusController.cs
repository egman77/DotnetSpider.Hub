using System;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Application.TaskStatus;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;

namespace DotnetSpider.Enterprise.Web.Controllers
{
	public class TaskStatusController : AppControllerBase
	{
		private readonly ITaskStatusAppService _taskStatusAppService;

		public TaskStatusController(ITaskStatusAppService taskStatusAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_taskStatusAppService = taskStatusAppService;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Query(PagingQueryTaskStatusInputDto input)
		{
			return DataResult(_taskStatusAppService.Query(input));
		}
	}
}
