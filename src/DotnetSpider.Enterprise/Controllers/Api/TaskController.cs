using DotnetSpider.Enterprise.Application.Task;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class TaskController : AppControllerBase
	{
		private readonly ITaskAppService _taskAppService;

		public TaskController(ITaskAppService taskAppService, IAppSession appSession, ICommonConfiguration commonConfiguration)
			: base(appSession, commonConfiguration)
		{
			_taskAppService = taskAppService;
		}

		[HttpGet]
		public IActionResult Find([FromQuery] PaginationQueryInput input)
		{
			var result = _taskAppService.Find(input);
			return Success(result);
		}

		[HttpPost]
		public IActionResult Create(CreateTaskInput item)
		{
			_taskAppService.Create(item);
			return Success();
		}

		[HttpDelete("{taskId}")]
		public IActionResult Delele(long taskId)
		{
			_taskAppService.Delete(taskId);
			return Success();
		}

		[HttpPut]
		public IActionResult Update(UpdateTaskInput item)
		{
			_taskAppService.Update(item);
			return Success();
		}

		[HttpGet("{taskId}")]
		[AllowAnonymous]
		public IActionResult Action(long taskId, [FromQuery] ActionType action)
		{
			_taskAppService.Control(taskId, action);
			return Success();
		}
	}
}
