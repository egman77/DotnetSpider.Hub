using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using DotnetSpider.Enterprise.Application.Pipeline;
using Microsoft.AspNetCore.Authorization;

namespace DotnetSpider.Enterprise.Controllers
{
	public class PipelineController : AppControllerBase
	{
		private readonly IPipelineAppService _pipelineAppService;

		public PipelineController(IAppSession appSession, ILoggerFactory loggerFactory,
			ICommonConfiguration commonConfiguration, IPipelineAppService pipelineAppService) : base(appSession, loggerFactory, commonConfiguration)
		{
			_pipelineAppService = pipelineAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Process()
		{
			return new JsonResult(_pipelineAppService.Process(Request.Body));
		}
	}
}
