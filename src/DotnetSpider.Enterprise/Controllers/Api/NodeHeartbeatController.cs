using DotnetSpider.Enterprise.Application.NodeHeartbeat;
using DotnetSpider.Enterprise.Application.NodeHeartbeat.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class NodeHeartbeatController : AppControllerBase
	{
		private readonly INodeHeartbeatAppService _heartbeatAppService;

		public NodeHeartbeatController(INodeHeartbeatAppService heartbeatAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration) : base(appSession, loggerFactory, commonConfiguration)
		{
			_heartbeatAppService = heartbeatAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Heartbeat([FromBody] NodeHeartbeatInput input)
		{
			CheckAuth();

			return Success(_heartbeatAppService.Create(input));
		}
	}
}
