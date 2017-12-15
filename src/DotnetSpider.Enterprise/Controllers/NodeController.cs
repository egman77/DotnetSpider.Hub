using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
{
	public class NodeController : AppControllerBase
	{
		private readonly INodeAppService _nodeAppService;

		public NodeController(INodeAppService nodeAppService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_nodeAppService = nodeAppService;
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Heartbeat([FromBody]NodeHeartbeatInputDto input)
		{
			if (ModelState.IsValid)
			{
				return new JsonResult(_nodeAppService.Heartbeat(input));
			}
			else
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public IActionResult Enable(string nodeId)
		{
			_nodeAppService.Enable(nodeId);
			return Success();
		}

		[HttpPost]
		public IActionResult Remove(string nodeId)
		{
			_nodeAppService.Remove(nodeId);
			return Success();
		}

		[HttpPost]
		public IActionResult Disable(string nodeId)
		{
			_nodeAppService.Disable(nodeId);
			return Success();
		}

		[HttpPost]
		public IActionResult Exit(string nodeId)
		{
			_nodeAppService.Exit(nodeId);
			return Success();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Query(PagingQueryInputDto input)
		{
			return DataResult(_nodeAppService.Query(input));
		}

		[HttpGet]
		public IActionResult Dashboard(string nodeId)
		{
			return View();
			//return DataResult(() => _nodeAppService.Dashboard(nodeId));
		}
	}
}
