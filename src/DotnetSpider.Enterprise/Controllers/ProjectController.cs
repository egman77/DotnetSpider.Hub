using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.Application.Project;
using DotnetSpider.Enterprise.Application.Project.Dtos;
using DotnetSpider.Enterprise.Domain;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Core.Configuration;

namespace DotnetSpider.Enterprise.Web.Controllers
{
	public class ProjectController : AppControllerBase
	{
		private readonly IProjectAppService _projectService;

		public ProjectController(IProjectAppService projectService, IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_projectService = projectService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult List()
		{
			return ActionResult(_projectService.ListProject);
		}

		[HttpPost]
		public IActionResult AddProject(ModifyProjectDto item)
		{
			if (ModelState.IsValid)
			{
				return ActionResult(() => { _projectService.AddProjcet(item); return item; });
			}
			else
			{
				return ErrorResult("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult RemoveProject(int projectId)
		{
			return ActionResult(() => { _projectService.RemoveProject(projectId); });
		}

		[HttpPost]
		public IActionResult ModifyProject(ModifyProjectDto item)
		{
			if (ModelState.IsValid)
			{
				return ActionResult(() => { _projectService.ModifyProject(item); });
			}
			else
			{
				return ErrorResult("参数不正确。");
			}
		}

		[HttpPost]
		public IActionResult GetStatusAndLogs(long buildId)
		{
			return ActionResult(_projectService.GetStatusAndLogs, buildId);
		}

		[HttpPost]
		public IActionResult EnableOrDisableProject(int projectId, bool enabled)
		{
			if (enabled)
			{
				return ActionResult(() => _projectService.EnableProject(projectId));
			}
			else
			{
				return ActionResult(() => _projectService.DisableProject(projectId));
			}
		}
	}
}
