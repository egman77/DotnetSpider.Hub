using DotnetSpider.Enterprise.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Application.Pipeline;
using Microsoft.AspNetCore.Authorization;
using System.IO;

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
			if (!IsAuth())
			{
				return BadRequest();
			}
			else
			{
				//var memoryStream = new MemoryStream();
				//Request.Body.CopyTo(memoryStream);

				return new JsonResult(_pipelineAppService.Process(Request.Body));
			}
		}
	}
}
