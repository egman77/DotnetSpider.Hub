using DotnetSpider.Enterprise.Application.TaskLog;
using DotnetSpider.Enterprise.Application.TaskLog.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	/// <summary>
	/// 因历史原因, 不建议路由修改为tasklog
	/// </summary>
	[Route("api/v1.0/log")]
	[Route("api/v1.0/tasklog")]
	public class TaskLogController : AppControllerBase
	{
		private readonly ITaskLogAppService _logAppService;

		public TaskLogController(ITaskLogAppService logAppService, IAppSession appSession, ICommonConfiguration commonConfiguration) : base(appSession, commonConfiguration)
		{
			_logAppService = logAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Create([FromBody] AddTaskLogInput input)
		{
			_logAppService.Add(input);
			return Success();
		}

		[HttpGet]
		public IActionResult Find(PaginationQueryInput input)
		{
			return Success(_logAppService.Find(input));
		}
	}
}
