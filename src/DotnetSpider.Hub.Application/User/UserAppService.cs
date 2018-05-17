using System.Linq;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Hub.Application.User
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
