using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
{
	public class TaskHistoryController : AppControllerBase
	{
		private readonly ITaskHistoryAppService _taskHistoryAppService;

		public TaskHistoryController(ITaskHistoryAppService taskHistoryAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_taskHistoryAppService = taskHistoryAppService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Query(PaginationQueryTaskHistoryInput input)
		{
			input.Sort = "desc";
			return DataResult(_taskHistoryAppService.Query(input));
		}
	}
}
