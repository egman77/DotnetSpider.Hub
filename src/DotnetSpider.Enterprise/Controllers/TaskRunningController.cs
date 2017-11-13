using DotnetSpider.Enterprise.Application.TaskRunning;
using DotnetSpider.Enterprise.Application.TaskRunning.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Controllers
{
	public class TaskRunningController : AppControllerBase
	{
		private readonly ITaskRunningAppService _taskRunningAppService;

		public TaskRunningController(ITaskRunningAppService taskRunningAppService, IAppSession appSession, ILoggerFactory loggerFactory,
			ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_taskRunningAppService = taskRunningAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Add([FromBody]AddTaskRunningInputDto input)
		{
			_taskRunningAppService.Add(input);
			return Ok();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Remove([FromBody]RemoveTaskRunningInputDto input)
		{
			_taskRunningAppService.Remove(input);
			return Ok();
		}
	}
}
