using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DotnetSpider.Enterprise.Application
{
	public abstract class AppServiceBase
	{
		protected readonly ICommonConfiguration Configuration;
		protected readonly UserManager<Domain.Entities.ApplicationUser> UserManager;
		protected readonly ApplicationDbContext DbContext;

		protected IAppSession Session { get; }

		protected AppServiceBase(ApplicationDbContext dbcontext, ICommonConfiguration configuration)
		{
			Session = DI.IocManager.GetRequiredService<IAppSession>();
			DbContext = dbcontext;
			Configuration = configuration;
			UserManager = DI.IocManager.GetRequiredService<UserManager<Domain.Entities.ApplicationUser>>();
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

		protected virtual string GetClientIp()
		{
			var httpContextAccessor = DI.IocManager.GetRequiredService<IHttpContextAccessor>();
			var ip = httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"];

			if (string.IsNullOrEmpty(ip))
			{
				ip = httpContextAccessor.HttpContext.Request.Headers["server.RemoteIpAddress"];
			}

			if (string.IsNullOrEmpty(ip))
			{
				throw new DotnetSpiderException("Cannot Detect Client Ip");
			}
			return ip;
		}

		protected bool CheckMyPermission(string claimName, bool throwException = true)
		{
			var r = UserManager.HasClaim(claimName);
			if (throwException)
			{
				if (!r)
				{
					throw new DotnetSpiderException($"Permission \"{claimName}\" required. Please contact Pa1Pa Service.");
				}
			}
			return r;
		}
	}
}
