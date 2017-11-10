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
			if (string.IsNullOrEmpty(input.NodeId))
			{
				return NotFound();
			}
			else
			{
				return new JsonResult(_nodeAppService.Heartbeat(input));
			}
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Enable(string nodeId)
		{
			_nodeAppService.EnableNode(nodeId);
			return Ok();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Disable(string nodeId)
		{
			_nodeAppService.DisableNode(nodeId);
			return Ok();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Dashboard(string nodeId)
		{
			return View();
		}
	}
}
