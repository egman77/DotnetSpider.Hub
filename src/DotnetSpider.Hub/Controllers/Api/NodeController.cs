using DotnetSpider.Hub.Application.Node;
using DotnetSpider.Hub.Application.Node.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class NodeController : BaseController
	{
		private readonly INodeAppService _nodeAppService;

		public NodeController(INodeAppService nodeAppService, IAppSession appSession, ICommonConfiguration commonConfiguration) : base(appSession, commonConfiguration)
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
