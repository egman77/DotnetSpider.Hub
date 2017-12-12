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
			if (!IsAuth())
			{
				return BadRequest();
			}
			else
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
		}

		[HttpPost]
		public IActionResult Enable(string nodeId)
		{
			return ActionResult(() => _nodeAppService.Enable(nodeId));
		}

		[HttpPost]
		public IActionResult Remove(string nodeId)
		{
			return ActionResult(() => _nodeAppService.Remove(nodeId));
		}

		[HttpPost]
		public IActionResult Disable(string nodeId)
		{
			return ActionResult(() => _nodeAppService.Disable(nodeId));
		}

		[HttpPost]
		public IActionResult Exit(string nodeId)
		{
			return ActionResult(() => _nodeAppService.Exit(nodeId));
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Query(PagingQueryInputDto input)
		{
			return ActionResult(() => _nodeAppService.Query(input));
		}

		[HttpGet]
		public IActionResult Dashboard(string nodeId)
		{
			return View();
		}
	}
}
