using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.Application.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DotnetSpider.Enterprise.Application
{
	public class SeedData
	{
		public static void Initialize(IServiceProvider serviceProvider, bool clear = false)
		{
			using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>(), null))
			{
				if (context.Users.Any())
				{
					return;   // 已经初始化过数据，直接返回
				}

				Clear(context);

				var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
				var roleManager = serviceProvider.GetService<RoleManager<IdentityRole<long>>>();

				var userAppService = serviceProvider.GetService<IUserAppService>();

				InitSuperAdmin(userManager, roleManager, context);
				InitData(context);
			}
		}

		private static void InitData(ApplicationDbContext context)
		{
			var random = new Random();
			for (int i = 0; i < 79; ++i)
			{
				Domain.Entities.Task task = new Domain.Entities.Task
				{
					ApplicationName = "Xbjrkj.DataCollection.Apps.dll",
					Cron = "* * * 1 *",
					IsEnabled = true,
					Developers = "沈威",
					Owners = "刘菲",
					Arguments = "-a:1,2",
					NodeCount = 1,
					NodeRunningCount = 0,
					Analysts = "刘菲",
					ProjectId = 1,
					Name = $"360指数采集",
					Version = "abcd"
				};
				context.Tasks.Add(task);
			}
			context.SaveChanges();
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
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole<long>> roleManager,
			ApplicationDbContext context)
		{
			//增加一个超级管理员用户
			var superAdmin = new ApplicationUser
			{
				IsActive = true,
				UserName = "service@dotnetspider.com",
				Email = "service@dotnetspider.com",
				EmailConfirmed = true,
				//IsActive = true,
				PhoneNumber = "17701696552"//,
										   //CreationTime = DateTime.Now,
										   //CreatorUserId = 0,
			};
			userManager.CreateAsync(superAdmin, "1qazZAQ!").Wait();
			context.SaveChanges();
		}
	}
}
