using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class NodeController : AppControllerBase
	{
		private readonly INodeAppService _nodeAppService;

		public NodeController(INodeAppService nodeAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration) : base(appSession, loggerFactory, commonConfiguration)
		{
			_nodeAppService = nodeAppService;
		}

		[HttpGet]
		public IActionResult Find([FromQuery]PaginationQueryInput input)
		{
			return Success(_nodeAppService.Query(input));
		}

		[HttpDelete("{nodeId}")]
		public IActionResult Delete(string nodeId)
		{
			_nodeAppService.Delete(nodeId);
			return Success();
		}

		[HttpGet("{nodeId}")]
		public IActionResult Action(string nodeId, [FromQuery] ActionType action)
		{
			_nodeAppService.Control(nodeId, action);
			return Success();
		}
	}
}
