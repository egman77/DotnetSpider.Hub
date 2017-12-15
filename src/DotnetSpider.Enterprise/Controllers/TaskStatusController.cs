using DotnetSpider.Enterprise.Application.TaskStatus;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
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
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Query(PagingQueryTaskStatusInputDto input)
		{
			input.Sort = "desc";
			return DataResult(_taskStatusAppService.Query(input));
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult AddOrUpdate([FromBody]AddOrUpdateTaskStatusInputDto input)
		{
			if (input == null)
			{
				return Forbid();
			}
			_taskStatusAppService.AddOrUpdate(input);
			return Ok();
		}
	}
}
