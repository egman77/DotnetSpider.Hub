using DotnetSpider.Hub.Application.TaskStatus;
using DotnetSpider.Hub.Application.TaskStatus.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class TaskStatusController : BaseController
	{
		private readonly ITaskStatusAppService _taskStatusAppService;

		public TaskStatusController(ITaskStatusAppService taskStatusAppService, ICommonConfiguration commonConfiguration) : base(commonConfiguration)
		{
			_taskStatusAppService = taskStatusAppService;
		}

		[HttpGet]
		public IActionResult Find([FromQuery]PaginationQueryTaskStatusInput input)
		{
			return Success(_taskStatusAppService.Query(input));
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
