using DotnetSpider.Enterprise.Application.Task;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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

		[HttpPost]
		[AllowAnonymous]
		public IActionResult ProcessCountChanged([FromBody]ProcessCountChangedInputDto input)
		{
			//string token, long taskId, bool isStart
			_taskAppService.ProcessCountChanged(input.TaskId, input.IsStart);
			return Ok();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Batches()
		{
			return View();
		}

		[HttpPost]
		public IActionResult GetList(PagingQueryTaskInputDto input)
		{
			input.Sort = "desc";
			var result = _taskAppService.GetList(input);
			return DataResult(result);
		}

		//[HttpPost]
		//public IActionResult GetVersions(QueryTaskVersionInputDto input)
		//{
		//	input.Sort = "desc";
		//	return ActionResult(() => _taskAppService.GetVersions(input));
		//}

		//[HttpPost]
		//public IActionResult SetVersion(long taskId, string version)
		//{
		//	return ActionResult(() => _taskAppService.SetVersion(taskId, version));
		//}

		[HttpPost]
		public IActionResult AddTask(TaskDto item)
		{
			if (ModelState.IsValid)
			{
				return ActionResult(() => { _taskAppService.AddTask(item); return item; });
			}
			else
			{
				return ErrorResult("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult ModifyTask(TaskDto item)
		{
			if (ModelState.IsValid)
			{
				return ActionResult(() => { _taskAppService.ModifyTask(item); });
			}
			else
			{
				return ErrorResult("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult GetRunningTasks()
		{
			return ActionResult(() => _taskAppService.GetRunningTasks());
		}

		[HttpGet]
		public IActionResult RunningTask()
		{
			return View();
		}

		[HttpPost]
		public IActionResult RunTask(long taskId)
		{
			return ActionResult(() => _taskAppService.RunTask(taskId));
		}

		[HttpPost]
		public IActionResult IsTaskRunning(long[] tasks)
		{
			return ActionResult(() => _taskAppService.IsTaskRunning(tasks));
		}

		[HttpPost]
		public IActionResult StopTask(string identity)
		{
			return ActionResult(() => { _taskAppService.StopTask(identity); });
		}

		[HttpPost]
		public IActionResult PauseTask(string identity)
		{
			return ActionResult(() => { _taskAppService.PauseTask(identity); });
		}

		[HttpPost]
		public IActionResult ResumeTask(string identity)
		{
			return ActionResult(() => { _taskAppService.ResumeTask(identity); });
		}

		[HttpGet]
		public IActionResult RunningLogs()
		{
			return View();
		}

		[HttpGet]
		public IActionResult RunningNodes()
		{
			return View();
		}

		[HttpPost]
		public IActionResult DeleteTask(long taskId)
		{
			return ActionResult(() => { _taskAppService.RemoveTask(taskId); });
		}

		[HttpPost]
		public IActionResult TaskRunning(string identity)
		{
			return ActionResult(_taskAppService.TaskRunning, identity);
		}
	}
}
