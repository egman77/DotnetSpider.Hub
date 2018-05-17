using DotnetSpider.Hub.Application.Pipeline;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers
{
	public class PipelineController : BaseController
	{
		private readonly IPipelineAppService _pipelineAppService;

		public PipelineController(IAppSession appSession,
			ICommonConfiguration commonConfiguration, IPipelineAppService pipelineAppService) : base(appSession, commonConfiguration)
		{
			_pipelineAppService = pipelineAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Process()
		{
			//return new JsonResult(_pipelineAppService.Process(Request.Body));
			return NoContent();
		}
	}
}
