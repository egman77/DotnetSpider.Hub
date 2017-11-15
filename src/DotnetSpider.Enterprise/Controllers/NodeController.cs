using System;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Web.Controllers
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
			return NotFound();
		}

		[HttpPost]
		public IActionResult Enable(string nodeId)
		{
			_nodeAppService.Enable(nodeId);
			return Ok();
		}

		[HttpPost]
		public IActionResult Disable(string nodeId)
		{
			_nodeAppService.Disable(nodeId);
			return Ok();
		}

		[HttpPost]
		public IActionResult Exit(string nodeId)
		{
			_nodeAppService.Exit(nodeId);
			return Ok();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult QueryNodes(PagingQueryInputDto input)
		{
			return ActionResult(() => _nodeAppService.QueryNodes(input));
		}

		[HttpGet]
		public IActionResult Dashboard(string nodeId)
		{
			return View();
		}
	}
}
