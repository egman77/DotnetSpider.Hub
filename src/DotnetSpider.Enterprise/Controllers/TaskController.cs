using DotnetSpider.Enterprise.Application.Task;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace DotnetSpider.Enterprise.Web.Controllers
{
	public class TaskController : AppControllerBase
	{
		private readonly ITaskAppService _taskAppService;

		public TaskController(ITaskAppService taskAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_taskAppService = taskAppService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Running()
		{
			return View();
		}

		[HttpGet]
		public IActionResult RunHistory()
		{
			return View();
		}

		#region API

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Running(PagingQueryInputDto input)
		{
			return DataResult(_taskAppService.Running(input));
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Fire([FromBody]SchedulerResponseObject response)
		{
			return ActionResult(() => { return _taskAppService.Fire(response.Id); });
		}

		[HttpPost]
		public IActionResult GetList(PagingQueryTaskInputDto input)
		{
			input.Sort = "desc";
			var result = _taskAppService.Query(input);
			return DataResult(result);
		}

		[HttpPost]
		public IActionResult Add(TaskDto item)
		{
			if (ModelState.IsValid)
			{
				return ActionResult(() => { _taskAppService.Add(item); return item; });
			}
			else
			{
				return ErrorResult("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult Modify(TaskDto item)
		{
			if (ModelState.IsValid)
			{
				return ActionResult(() => { _taskAppService.Modify(item); });
			}
			else
			{
				return ErrorResult("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult Run(long taskId)
		{
			return ActionResult(() => _taskAppService.Run(taskId));
		}

		[HttpPost]
		public IActionResult Exit(string identity)
		{
			return ActionResult(() => { _taskAppService.Exit(identity); });
		}

		[HttpPost]
		public IActionResult Remove(long taskId)
		{
			return ActionResult(() => { _taskAppService.Remove(taskId); });
		}

		[HttpPost]
		public IActionResult Disable(long taskId)
		{
			return ActionResult(() => { _taskAppService.Disable(taskId); });
		}

		[HttpPost]
		public IActionResult Enable(long taskId)
		{
			return ActionResult(() => { _taskAppService.Enable(taskId); });
		}

		[HttpPost]
		public IActionResult QueryRunHistory(PagingQueryTaskHistoryInputDto input)
		{
			input.Sort = "desc";
			var result = _taskAppService.QueryRunHistory(input);
			return DataResult(result);
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult IncreaseRunning([FromBody]TaskIdInputDto input)
		{
			_taskAppService.IncreaseRunning(input);
			return Ok();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult ReduceRunning([FromBody]TaskIdInputDto input)
		{
			_taskAppService.ReduceRunning(input);
			return Ok();
		}

		[HttpPost]
		public IActionResult GetTask(long taskId)
		{
			return ActionResult(_taskAppService.GetTask, taskId);
		}

		#endregion
	}
}
