using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.Application.User
{
	public class UserAppService : AppServiceBase, IUserAppService
	{
		public UserAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
		}

		public ApplicationUser GetUserByEmailAddress(string emailAddress)
		{
			return DbContext.Users.FirstOrDefault(u => u.NormalizedEmail == emailAddress.ToUpper());
		}

		public ApplicationUser GetUserById(long userId)
		{
			return DbContext.Users.FirstOrDefault(u => u.Id == userId);
		}

		public ApplicationUser GetUserByName(string userName)
		{
			return DbContext.Users.FirstOrDefault(u => u.NormalizedUserName == userName.ToUpper());
		}

		public ApplicationUser GetUserByNameOrEmailAddress(string userNameOrEmailAddress)
		{
			var user = DbContext.Users.FirstOrDefault(u => u.NormalizedUserName == userNameOrEmailAddress.ToUpper());
			if (user == null)
			{
				user = user = DbContext.Users.FirstOrDefault(u => u.NormalizedEmail == userNameOrEmailAddress.ToUpper());
			}
			return user;
		}
	}
}
