using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace DotnetSpider.Hub.Application.Config
{
	public class ConfigAppService : AppServiceBase, IConfigAppService
	{
		protected ConfigAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, UserManager<ApplicationUser> userManager) : base(dbcontext, configuration, userManager)
		{
		}

		public string Get(string name)
		{
			var config = DbContext.Config.FirstOrDefault(c => c.Name == name);
			Logger.Information($"Get config: {name}: {config?.Value}.");
			return config?.Value;
		}
	}
}
