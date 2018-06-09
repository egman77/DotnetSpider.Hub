using DotnetSpider.Hub.Application.Task;
using DotnetSpider.Hub.Application.Task.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class TaskController : BaseController
	{
		private readonly ITaskAppService _taskAppService;

		public TaskController(ITaskAppService taskAppService, ICommonConfiguration commonConfiguration)
			: base(commonConfiguration)
		{
			_taskAppService = taskAppService;
		}

		[HttpGet]
		public IActionResult Query([FromQuery] PaginationQueryTaskInput input)
		{
			var result = _taskAppService.Query(input);
			return Success(result);
		}

		[HttpPost]
		public IActionResult Create(CreateTaskInput item)
		{
			_taskAppService.Create(item);
			return Success();
		}

		[HttpDelete("{taskId}")]
		public IActionResult Delele(string taskId)
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
		public IActionResult Action(string taskId, [FromQuery] ActionType action = ActionType.Query)
		{
			if (action == ActionType.Query)
			{
				return Success(_taskAppService.GetTask(taskId));
			}
			else
			{
				_taskAppService.Control(taskId, action);
				return Success();
			}
		}
	}
}
