using DotnetSpider.Hub.Application.NodeHeartbeat;
using DotnetSpider.Hub.Application.NodeHeartbeat.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class NodeHeartbeatController : BaseController
	{
		private readonly INodeHeartbeatAppService _heartbeatAppService;

		public NodeHeartbeatController(INodeHeartbeatAppService heartbeatAppService, IAppSession appSession, ICommonConfiguration commonConfiguration) : base(appSession, commonConfiguration)
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
