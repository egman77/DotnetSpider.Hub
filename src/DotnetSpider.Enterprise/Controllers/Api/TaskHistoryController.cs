using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class TaskHistoryController : AppControllerBase
	{
		private readonly ITaskHistoryAppService _taskHistoryAppService;


		public TaskHistoryController(ITaskHistoryAppService taskHistoryAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration) : base(appSession, loggerFactory, commonConfiguration)
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
