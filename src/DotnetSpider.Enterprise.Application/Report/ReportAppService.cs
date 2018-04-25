using DotnetSpider.Enterprise.Application.Report.Dtos;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using System.Linq;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Dapper;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.Application.Report
{
	public class ReportAppService : AppServiceBase, IReportAppService
	{
		private readonly INodeAppService _nodeAppService;
		private readonly long G = 1024 * 1024;

		public ReportAppService(INodeAppService nodeAppService, ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_nodeAppService = nodeAppService;
		}

		public dynamic Query(FilterQueryInput input)
		{
			var filters = input.ToDictionary();
			if (!filters.ContainsKey("type"))
			{
				return null;
			}
			switch (filters["type"])
			{
				case "homedashboad":
					{
						return CalculateHomeDashboardDto();
					}
				default:
					{
						return null;
					}
			}
		}

		private HomePageDashboardDto CalculateHomeDashboardDto()
		{
			HomePageDashboardDto output = new HomePageDashboardDto();

			long storageSize = 0;


			using (var conn = new SqlConnection(Configuration.MsSqlConnectionString))
			{
				var dbSpaceUsed = conn.Query<DbSpaceUsedDto>(" exec  sp_spaceused 'TaskLog'").First();
				storageSize = long.Parse(dbSpaceUsed.Reserved.Replace("KB", ""));
			}


			var nodes = _nodeAppService.Query(new PaginationQueryInput { Page = 1, Size = 10 });
			output.NodeTotalCount = (int)nodes.Total;
			output.NodeOnlineCount = _nodeAppService.GetOnlineNodeCount();
			output.Nodes = nodes.Result;
			output.TaskCount = DbContext.Task.Count(t => !t.IsDeleted);
			output.RunningTaskCount = DbContext.Task.Count(t => t.IsRunning);
			output.LogSize = storageSize > G ? storageSize / G + "G" : storageSize / 1024 + "M";
			return output;
		}
	}
}
