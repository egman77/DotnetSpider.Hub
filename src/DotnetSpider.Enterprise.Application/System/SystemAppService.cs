using DotnetSpider.Enterprise.Application.Hangfire;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace DotnetSpider.Enterprise.Application.System
{
	public class SystemAppService : AppServiceBase, ISystemAppService
	{
		private readonly IHangfireAppService _hangfireAppService;
		private readonly IMessageAppService _messageAppService;
		private readonly INodeAppService _nodeAppService;

		public const string ScanRunningTaskName = "System.DotnetSpider.ScanRunningTask";

		public SystemAppService(INodeAppService nodeAppService, IMessageAppService messageAppService, IHangfireAppService hangfireAppService,
			ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_hangfireAppService = hangfireAppService;
			_nodeAppService = nodeAppService;
			_messageAppService = messageAppService;
		}

		public void Register()
		{
			Domain.Entities.Task scanRunningTask = DbContext.Task.FirstOrDefault(t => t.Name.StartsWith(ScanRunningTaskName));
			if (scanRunningTask == null)
			{
				scanRunningTask = new Domain.Entities.Task
				{
					ApplicationName = "DotnetSpider.Enterprise",
					Cron = $"0/15 * * * *",
					IsEnabled = true,
					IsDeleted = true,
					Developers = "DotnetSpider",
					Owners = "DotnetSpider",
					Arguments = "",
					NodeCount = 1,
					NodeRunningCount = 0,
					Name = ScanRunningTaskName,
					Version = "0001",
					NodeType = 1
				};
				DbContext.Task.Add(scanRunningTask);
				DbContext.SaveChanges();
			}
			_hangfireAppService.AddOrUpdateHangfireJob(scanRunningTask.Id.ToString(), "0/15 * * * *");
		}

		public void Execute(string name, string arguments)
		{
			switch (name)
			{
				case ScanRunningTaskName:
					{
						ScanRunningTask();
						break;
					}
			}
		}

		private void ScanRunningTask()
		{
			var runningTasks = DbContext.Task.Where(t => t.IsRunning).ToList();
			foreach (var task in runningTasks)
			{
				var id = task.Id;
				var status = DbContext.TaskStatus.Where(ts => ts.TaskId == id).OrderByDescending(ts => ts.LastModificationTime).FirstOrDefault();
				if (status != null)
				{
					if (status.LastModificationTime.Value != null)
					{
						if ((DateTime.Now - status.LastModificationTime.Value).TotalSeconds > 3600)
						{
							TaskUtil.ExitTask(_nodeAppService, _messageAppService, task, Logger);
						}
					}
					else
					{
						if ((DateTime.Now - status.CreationTime).TotalSeconds > 3600)
						{
							TaskUtil.ExitTask(_nodeAppService, _messageAppService, task, Logger);
						}
					}
				}
			}
			DbContext.SaveChanges();
		}
	}
}
