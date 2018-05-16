using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class TaskHistoryController : AppControllerBase
	{
		private readonly ITaskHistoryAppService _taskHistoryAppService;


		public TaskHistoryController(ITaskHistoryAppService taskHistoryAppService, IAppSession appSession, ICommonConfiguration commonConfiguration) : base(appSession, commonConfiguration)
		{
			_taskHistoryAppService = taskHistoryAppService;
		}

		[HttpGet]
		public IActionResult Find(PaginationQueryInput input)
		{
			return Success(_taskHistoryAppService.Find(input));
		}
	}
}
