using DotnetSpider.Hub.Application.TaskLog;
using DotnetSpider.Hub.Application.TaskLog.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	/// <summary>
	/// 因历史原因, 不建议路由修改为tasklog
	/// </summary>
	[Route("api/v1.0/log")]
	[Route("api/v1.0/tasklog")]
	public class TaskLogController : BaseController
	{
		private readonly ITaskLogAppService _logAppService;

		public TaskLogController(ITaskLogAppService logAppService, ICommonConfiguration commonConfiguration) : base(commonConfiguration)
		{
			_logAppService = logAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Create([FromBody] params AddTaskLogInput[] input)
		{
			_logAppService.Add(input);
			return Success();
		}

		[HttpGet]
		public IActionResult Find(PaginationQueryTaskLogInput input)
		{
			return Success(_logAppService.Find(input));
		}
	}
}
