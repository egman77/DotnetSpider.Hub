using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace DotnetSpider.Enterprise.Application.User
{
	public class UserAppService : AppServiceBase, IUserAppService
	{
		public UserAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{

		}

		public Domain.Entities.ApplicationUser GetUserByEmailAddress(string emailAddress)
		{
			return DbContext.Users.FirstOrDefault(u => u.NormalizedEmail == emailAddress.ToUpper());
		}

		public Domain.Entities.ApplicationUser GetUserById(long userId)
		{
			return DbContext.Users.FirstOrDefault(u => u.Id == userId);
		}

		public Domain.Entities.ApplicationUser GetUserByName(string userName)
		{
			return DbContext.Users.FirstOrDefault(u => u.NormalizedUserName == userName.ToUpper());
		}

		public Domain.Entities.ApplicationUser GetUserByNameOrEmailAddress(string userNameOrEmailAddress)
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
