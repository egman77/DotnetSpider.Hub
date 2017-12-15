using DotnetSpider.Enterprise.Application.Log;
using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
{
	public class LogController : AppControllerBase
	{
		private readonly ILogAppService _logAppService;

		public LogController(IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration,
			ILogAppService logAppService)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_logAppService = logAppService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		[AllowAnonymous]
		public IActionResult Submit([FromBody] LogInputDto input)
		{
			_logAppService.Sumit(input);
			return Success();
		}

		[HttpPost]
		public IActionResult Query(PagingLogInputDto input)
		{
			return DataResult(_logAppService.Query(input));
		}

		[HttpPost]
		public IActionResult Clear()
		{
			_logAppService.Clear();
			return Success();
		}
	}
}
