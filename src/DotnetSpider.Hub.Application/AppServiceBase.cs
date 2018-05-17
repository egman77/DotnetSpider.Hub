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
		protected readonly IAppSession Session;
		protected readonly ILogger Logger;

		protected AppServiceBase(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<ApplicationUser> userManager)
		{
			DbContext = dbcontext;
			Configuration = configuration;
			UserManager = userManager;
			Session = appSession;
			Logger = Log.ForContext(GetType());
		}

		protected virtual ApplicationUser GetCurrentUser()
		{
			if (Session.UserId != null)
			{
				var userId = Session.UserId.Value;
				var user = UserManager.GetUserById(userId);
				if (user == null)
				{
					throw new DotnetSpiderException("There is no current user!");
				}

				return user;
			}

			return null;
		}
	}
}
