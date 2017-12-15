using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DotnetSpider.Enterprise.Application
{
	public abstract class AppServiceBase
	{
		protected readonly ICommonConfiguration Configuration;
		protected readonly UserManager<Domain.Entities.ApplicationUser> UserManager;
		protected readonly ApplicationDbContext DbContext;
		protected readonly IAppSession Session;
		protected readonly ILogger Logger;

		protected AppServiceBase(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager, ILoggerFactory loggerFactory)
		{
			DbContext = dbcontext;
			Configuration = configuration;
			UserManager = userManager;
			Session = appSession;
			Logger = loggerFactory.CreateLogger(GetType());
		}

		protected virtual Domain.Entities.ApplicationUser GetCurrentUser()
		{
			var userId = Session.UserId.Value;
			var user = UserManager.GetUserById(userId);
			if (user == null)
			{
				throw new DotnetSpiderException("There is no current user!");
			}

			return user;
		}

		protected bool IsAuth()
		{
			if (!Configuration.AuthorizeApi)
			{
				return true;
			}
			if (Session.Request.Headers.ContainsKey("DotnetSpiderToken"))
			{
				var token = Session.Request.Headers["DotnetSpiderToken"].ToString();
				return Configuration.Tokens.Contains(token);
			}
			else
			{
				return false;
			}
		}
	}
}
