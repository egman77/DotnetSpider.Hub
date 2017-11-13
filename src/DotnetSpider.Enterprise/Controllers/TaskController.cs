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
			var result = _taskAppService.GetList(input);
			return DataResult(result);
		}

		[HttpPost]
		public IActionResult Add(TaskDto item)
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
		public IActionResult Modify(TaskDto item)
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
		public IActionResult Run(long taskId)
		{
			return ActionResult(() => _taskAppService.RunTask(taskId));
		}

		[HttpPost]
		public IActionResult Exit(string identity)
		{
			return ActionResult(() => { _taskAppService.StopTask(identity); });
		}

		[HttpPost]
		public IActionResult Remove(long id)
		{
			return ActionResult(() => { _taskAppService.RemoveTask(id); });
		}

		[HttpPost]
		public IActionResult Disable(long id)
		{
			return ActionResult(() => { _taskAppService.Disable(id); });
		}

		[HttpPost]
		public IActionResult Enable(long id)
		{
			return ActionResult(() => { _taskAppService.Enable(id); });
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
	}
}
