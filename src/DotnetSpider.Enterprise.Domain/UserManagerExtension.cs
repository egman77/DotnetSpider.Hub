using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotnetSpider.Enterprise.Domain
{
	public static class UserManagerExtension
	{
		private static IHttpContextAccessor Accessor;

		static UserManagerExtension()
		{
			Accessor = DI.IocManager.GetRequiredService<IHttpContextAccessor>();
		}

		public static ApplicationUser GetUserById(this UserManager<ApplicationUser> userManager, long id)
		{
			return userManager.Users.FirstOrDefault(u => u.Id == id);
		}

		public static bool HasClaim(this UserManager<ApplicationUser> userManager, string claimName)
		{
			return Accessor.HttpContext.User.Claims.Any(c => c.Value == claimName);
		}
	}
}
