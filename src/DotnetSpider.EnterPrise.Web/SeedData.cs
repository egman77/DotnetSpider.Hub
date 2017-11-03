using DotnetSpider.EnterPrise.Web.Data;
using DotnetSpider.EnterPrise.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSpider.EnterPrise.Web
{
	public class SeedData
	{
		public static void Initialize(IServiceProvider serviceProvider, bool clear = false)
		{
			

				var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
				var roleManager = serviceProvider.GetService<RoleManager<IdentityRole<long>>>();

				InitSuperAdmin(userManager);
			
		}

		private static void Clear(ApplicationDbContext context)
		{
			var roleClaims = context.RoleClaims.ToList();
			foreach (var d in roleClaims)
			{
				context.RoleClaims.Remove(d);
			}

			var roles = context.Roles.ToList();
			foreach (var d in roles)
			{
				context.Roles.Remove(d);
			}

			var users = context.Users.ToList();
			foreach (var d in users)
			{
				context.Users.Remove(d);
			}

			context.SaveChanges();
		}

		private static void InitSuperAdmin(
			UserManager<ApplicationUser> userManager)
		{
			//增加一个超级管理员用户
			var superAdmin = new ApplicationUser
			{
				UserName = "service@dotnetspider.com",
				Email = "service@dotnetspider.com",
				EmailConfirmed = true,
				//IsActive = true,
				PhoneNumber = "17701696552"//,
										   //CreationTime = DateTime.Now,
										   //CreatorUserId = 0,
			};
			userManager.CreateAsync(superAdmin, "1qazZAQ!").Wait();
			//context.SaveChanges();
		}
	}
}
