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

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Dashboard(string id)
		{
			ViewBag.AgentId = id;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult GetCurrentNodeInfo()
		{
			return ActionResult(() => _nodeAppService.GetCurrentNodeInfo());
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult GetNodeDetail(string id)
		{
			return ActionResult(() => _nodeAppService.GetNodeDetail(id));
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult EnableNode(NodeEnable input)
		{
			if (ModelState.IsValid)
			{
				return ActionResult(() => _nodeAppService.EnableNode(input));
			}
			else
			{
				throw new AppException("Invalid Params.");
			}
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult GetLog(GetLogInput input)
		{
			if (ModelState.IsValid)
			{
				input.Sort = "desc";
				return ActionResult(() => _nodeAppService.GetLog(input));
			}
			else
			{
				throw new AppException("Invalid Params.");
			}
		}
	}
}
