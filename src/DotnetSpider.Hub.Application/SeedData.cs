using System;
using System.Linq;
using DotnetSpider.Hub.Application.System;
using DotnetSpider.Hub.Application.Task;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Application
{
	public class SeedData : ISeedData
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITaskAppService _taskAppService;
		private readonly ISystemAppService _systemAppService;

		public SeedData(ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			ITaskAppService taskAppService,
			ISystemAppService systemAppService, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
			_userManager = userManager;
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
					Cron = $"* * * * *",
					IsEnabled = true,
					Developers = "沈威",
					Owners = "刘菲",
					Arguments = "-a:1,2",
					NodeCount = 1,
					NodeRunningCount = 0,
					Analysts = "刘菲",
					Name = $"360指数采集",
					Package = "abcd",
					Os = "windows",
					NodeType = "default"
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
			_context.Roles.Add(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN", Id = Guid.NewGuid().ToString("N"), ConcurrencyStamp = Guid.NewGuid().ToString() });
			//增加一个超级管理员用户
			var superAdmin = new ApplicationUser
			{
				UserName = "hdh@cvoit.com", //这个必须写成邮箱名,否则登录界面验证会导致无法正确传向后台
				Email = "hdh@cvoit.com",
				EmailConfirmed = true,
				PhoneNumber = "18701698558"
			};

            //要复杂密码(startup.cs的ConfigureServices方法中有指定),否则创建不成功!
            _userManager.CreateAsync(superAdmin, "1qazZAQ!").Wait();

			_userManager.AddToRoleAsync(superAdmin, "Admin").Wait();
			_context.SaveChanges();
		}
	}
}
