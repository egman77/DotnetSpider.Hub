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
			IAppSession appSession, UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
		}

		public void CreateDatabaseAndTable(string database, string table, string[] columns)
		{

		}

		public int Process(string database, string table, string[] values)
		{
			throw new NotImplementedException();
		}
	}
}
