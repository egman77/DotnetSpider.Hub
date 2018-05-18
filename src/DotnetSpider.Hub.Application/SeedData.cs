using System;
using System.Linq;
using DotnetSpider.Hub.Application.System;
using DotnetSpider.Hub.Application.Task;
using DotnetSpider.Hub.Application.User;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Application
{
	public class SeedData
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserAppService _userAppService;
		private readonly ITaskAppService _taskAppService;
		private readonly ISystemAppService _systemAppService;

		public SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserAppService userAppService, ITaskAppService taskAppService,
		ISystemAppService systemAppService, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
			_userManager = userManager;
			_userAppService = userAppService;
			_taskAppService = taskAppService;
			_systemAppService = systemAppService;
		}

		public void Init()
		{
			_context.Database.Migrate();

			var clearDatabase = _configuration["clear"] == "true";
			if (clearDatabase)
			{
				Clear();
			}

			if (_context.Users.Any())
			{
				return; // 已经初始化过数据，直接返回
			}

			CreateAdmin();

			var initTestData = _configuration["initestdata"] == "true";

			if (initTestData)
			{
				InitTestData();
			}

			_systemAppService.Register();

			var upgradeScheduler = _configuration["upgradescheduler"] == "true";
			if (upgradeScheduler)
			{
				_systemAppService.UpgradeScheduler();
			}
		}

		private void InitTestData()
		{
			var random = new Random();
			for (int i = 0; i < 100; ++i)
			{
				Core.Entities.Task task = new Core.Entities.Task
				{
					ApplicationName = "dotnet",
					Cron = $"* * * {i} *",
					IsEnabled = true,
					Developers = "沈威",
					Owners = "刘菲",
					Arguments = "-a:1,2",
					NodeCount = 1,
					NodeRunningCount = 0,
					Analysts = "刘菲",
					Name = $"360指数采集",
					Version = "abcd",
					NodeType = 1
				};
				_context.Task.Add(task);
			}
			_context.SaveChanges();
		}

		private void Clear()
		{
			var roleClaims = _context.RoleClaims;
			foreach (var d in roleClaims)
			{
				_context.RoleClaims.Remove(d);
			}

			var roles = _context.Roles;
			foreach (var d in roles)
			{
				_context.Roles.Remove(d);
			}

			var users = _context.Users;
			foreach (var d in users)
			{
				_context.Users.Remove(d);
			}
			_context.SaveChanges();
		}

		private void CreateAdmin()
		{
			//增加一个超级管理员用户
			var superAdmin = new ApplicationUser
			{
				IsActive = true,
				UserName = "service@dotnetspider.org",
				Email = "service@dotnetspider.org",
				EmailConfirmed = true,
				PhoneNumber = "17701696558"
			};
			_userManager.CreateAsync(superAdmin, "1qazZAQ!").Wait();
			_context.SaveChanges();
		}
	}
}
