using DotnetSpider.Enterprise.Application.Report.Dtos;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using MongoDB.Driver;
using System.Linq;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Application.Report
{
	public class ReportAppService : AppServiceBase, IReportAppService
	{
		private readonly INodeAppService _nodeAppService;
		private readonly long G = 1024 * 1024;

		public ReportAppService(INodeAppService nodeAppService, ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager, ILoggerFactory loggerFactory)
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
			var client = new MongoClient(Configuration.LogMongoConnectionString);

			var database = client.GetDatabase("dotnetspider");

			BsonDocumentCommand<BsonDocument> command = new BsonDocumentCommand<BsonDocument>(new BsonDocument { { "dbStats", 1 }, { "scale", 1024 } });
			var result = database.RunCommand(command);
			var storageSize = result.GetValue("storageSize").ToInt64();

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
