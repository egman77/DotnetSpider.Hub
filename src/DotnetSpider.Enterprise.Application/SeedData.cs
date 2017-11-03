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
			using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
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
			var project = new Domain.Entities.Project
			{
				Client = "刘菲",
				CreationTime = DateTime.Now,
				CreatorUserId = 1,
				IsEnabled = true,
				Name = "DotnetSpider",
				Executive = "沈威",
				Note = "备注"
			};
			context.Projects.Add(project);
			context.SaveChanges();

			var random = new Random();
			for (int i = 0; i < 79; ++i)
			{
				Domain.Entities.Task task = new Domain.Entities.Task
				{
					Framework = "NET CORE",
					AssemblyName = "Xbjrkj.DataCollection.Apps.dll",
					Cron = "* * * 1 *",
					IsEnabled = true,
					Programmer = "沈威",
					Executive = "沈威",
					ExtraArguments = "-a:1,2",
					BuildTime = DateTime.Now.AddDays(-1),
					Client = "",
					NodesCount = 1,
					ProjectId = 1,
					TaskName = "360Index",
					Name = $"360指数采集",
					Version = DateTime.Now.ToString("yyyyMMddhhmmss"),
					Project = project
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
