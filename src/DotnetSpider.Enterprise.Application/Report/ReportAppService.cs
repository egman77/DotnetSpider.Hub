using DotnetSpider.Enterprise.Application.Report.Dtos;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AutoMapper;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Enterprise.Application.Report
{
	public class ReportAppService : AppServiceBase, IReportAppService
	{
		private readonly INodeAppService _nodeAppService;

		public ReportAppService(INodeAppService nodeAppService, ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
			_nodeAppService = nodeAppService;
		}

		public HomePageDashboardOutputDto GetHomePageDashboard()
		{
			HomePageDashboardOutputDto output = new HomePageDashboardOutputDto();
			var client = new MongoClient(Configuration.LogMongoConnectionString);
			var database = client.GetDatabase("dotnetspider");
			//output.LogCount=
			var nodes = DbContext.Node.ToList();
			output.NodeTotalCount = nodes.Count;
			output.NodeOnlineCount = nodes.Count(t => t.IsOnline);
			output.Nodes = _nodeAppService.Query(new Domain.PagingQueryInputDto { Page = 1, Size = 10 }).Result;
			output.TaskCount = DbContext.Task.Count(t => !t.IsDeleted);
			output.RunningTaskCount = DbContext.Task.Count(t => t.IsRunning);
			return output;
		}
	}
}
