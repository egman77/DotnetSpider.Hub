using System;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Hub.Application.Pipeline
{
	public class PipelineAppService : AppServiceBase, IPipelineAppService
	{
		public PipelineAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, userManager)
		{
		}

		public void CreateDatabaseAndTable(string database, string table, string[] columns)
		{

		}

		public int Process(string database, string table, string[] values)
		{
			return 0;
		}
	}
}
