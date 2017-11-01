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
				Framework = ".NET 45",
				GitUrl = "https://github.com/zlzforever/DotnetSpider.git",
				IntervalPath = "",
				IsEnabled = true,
				Name = "DotnetSpider"
			};
			context.Projects.Add(project);
			context.SaveChanges();

			for (int i = 0; i < 23; ++i)
			{
				var project1 = new Domain.Entities.Project
				{
					Framework = ".NET 45",
					GitUrl = "https://github.com/zlzforever/DotnetSpider.git" + i,
					IntervalPath = "",
					IsEnabled = true,
					Name = "DotnetSpider" + i
				};
				context.Projects.Add(project1);
			}
			context.SaveChanges();

			var random = new Random();
			for (int i = 0; i < 79; ++i)
			{
				Domain.Entities.Task task = new Domain.Entities.Task
				{
					Arguments = $"-s:spider{i}",
					CountOfNodes = random.Next(1, 10),
					Cron = "",
					IsEnabled = true,
					Name = $"spider{i}",
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
				UserName = "service@dotnetspider.com",
				Email = "service@dotnetspider.com",
				EmailConfirmed = true,
				IsActive = true,
				PhoneNumber = "17701696552",
				CreationTime = DateTime.Now,
				CreatorUserId = 0,
			};
			userManager.CreateAsync(superAdmin, "66666666Rr%").Wait();
			context.SaveChanges();
		}
	}
}
