using DotnetSpider.Enterprise.Application.TaskStatus;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class TaskStatusController : AppControllerBase
	{
		private readonly ITaskStatusAppService _taskStatusAppService;

		public TaskStatusController(ITaskStatusAppService taskStatusAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration) : base(appSession, loggerFactory, commonConfiguration)
		{
			_taskStatusAppService = taskStatusAppService;
		}

		[HttpGet]
		public IActionResult Find([FromQuery]PaginationQueryInput input)
		{
			return Success(_taskStatusAppService.Find(input));
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult AddOrUpdate([FromBody]AddOrUpdateTaskStatusInput input)
		{
			if (input == null)
			{
				return NoContent();
			}
			_taskStatusAppService.AddOrUpdate(input);
			return Ok();
		}
	}
}
