using System;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Enterprise.Web.Controllers
{
	public class NodeController : AppControllerBase
	{
		private readonly INodeAppService _nodeAppService;

		public NodeController(INodeAppService nodeAppService)
		{
			_nodeAppService = nodeAppService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Dashboard(string id)
		{
			ViewBag.AgentId = id;
			return View();
		}

		[HttpPost]
		public IActionResult GetCurrentNodeInfo()
		{
			return ActionResult(() => _nodeAppService.GetCurrentNodeInfo());
		}

		[HttpPost]
		public IActionResult GetNodeDetail(string id)
		{
			return ActionResult(() => _nodeAppService.GetNodeDetail(id));
		}

		[HttpPost]
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
