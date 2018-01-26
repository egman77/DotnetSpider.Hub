using DotnetSpider.Enterprise.Application.Log;
using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class LogController : AppControllerBase
	{
		private readonly ILogAppService _logAppService;

		public LogController(ILogAppService logAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration) : base(appSession, loggerFactory, commonConfiguration)
		{
			_logAppService = logAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Create([FromBody] AddLogInput input)
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
