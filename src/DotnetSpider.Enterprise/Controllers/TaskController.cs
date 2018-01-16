using DotnetSpider.Enterprise.Application.Task;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
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
		public IActionResult Running(PaginationQueryInput input)
		{
			return DataResult(_taskAppService.QueryRunning(input));
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Fire(long data)
		{
			_taskAppService.Run(data);
			return Success();
		}

		[HttpPost]
		public IActionResult Query(PaginationQueryTaskInput input)
		{
			input.Sort = "desc";
			var result = _taskAppService.Query(input);
			return DataResult(result);
		}

		[HttpPost]
		public IActionResult Add(AddTaskInput item)
		{
			if (ModelState.IsValid)
			{
				_taskAppService.Add(item);
				return Success();
			}
			else
			{
				throw new DotnetSpiderException("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult Modify(ModifyTaskInput item)
		{
			if (ModelState.IsValid)
			{
				_taskAppService.Modify(item); ;
				return Success();
			}
			else
			{
				throw new DotnetSpiderException("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult Run(long taskId)
		{
			_taskAppService.Run(taskId);
			return Success();
		}

		[HttpPost]
		public IActionResult Exit(long taskId)
		{
			_taskAppService.Exit(taskId);
			return Success();
		}

		[HttpPost]
		public IActionResult Remove(long taskId)
		{
			_taskAppService.Remove(taskId);
			return Success();
		}

		[HttpPost]
		public IActionResult Disable(long taskId)
		{
			_taskAppService.Disable(taskId);
			return Success();
		}

		[HttpPost]
		public IActionResult Enable(long taskId)
		{
			_taskAppService.Enable(taskId);
			return Success();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult IncreaseRunning([FromBody]TaskIdInput input)
		{
			_taskAppService.IncreaseRunning(input);
			return Success();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult ReduceRunning([FromBody]TaskIdInput input)
		{
			_taskAppService.ReduceRunning(input);
			return Success();
		}

		[HttpPost]
		public IActionResult Get(long taskId)
		{
			return DataResult(_taskAppService.Get(taskId));
		}

		#endregion
	}
}
