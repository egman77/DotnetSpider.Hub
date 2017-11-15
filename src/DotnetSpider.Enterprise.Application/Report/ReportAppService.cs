using DotnetSpider.Enterprise.Application.Report.Dtos;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AutoMapper;
using DotnetSpider.Enterprise.Application.Node.Dto;

namespace DotnetSpider.Enterprise.Application.Report
{
	public class ReportAppService : AppServiceBase, IReportAppService
	{
		public ReportAppService(ApplicationDbContext dbcontext) : base(dbcontext)
		{

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
			output.Nodes = Mapper.Map<List<NodeOutputDto>>(nodes);
			output.TaskCount = DbContext.Task.Count(t => !t.IsDeleted);
			output.RunningTaskCount = DbContext.Task.Count(t => t.IsRunning);
			return output;
		}
	}
}
