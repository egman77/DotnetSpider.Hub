using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace DotnetSpider.Hub.Application
{
	public abstract class AppServiceBase
	{
		protected readonly ICommonConfiguration Configuration;
		protected readonly UserManager<ApplicationUser> UserManager;
		protected readonly ApplicationDbContext DbContext;
		protected readonly ILogger Logger;

		protected AppServiceBase(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager)
		{
			DbContext = dbcontext;
			Configuration = configuration;
			UserManager = userManager;
			Logger = Log.ForContext(GetType());
		}
	}
}
