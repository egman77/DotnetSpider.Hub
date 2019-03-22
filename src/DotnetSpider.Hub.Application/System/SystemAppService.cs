using System;
using System.Linq;
using DotnetSpider.Hub.Application.Message;
using DotnetSpider.Hub.Application.Node;
using DotnetSpider.Hub.Application.Scheduler;
using DotnetSpider.Hub.Application.Scheduler.Dtos;
using DotnetSpider.Hub.Application.Task;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DotnetSpider.Hub.Application.System
{
	public class SystemAppService : AppServiceBase, ISystemAppService
	{
		private readonly ISchedulerAppService _schedulerAppService;
		private readonly IMessageAppService _messageAppService;
		private readonly INodeAppService _nodeAppService;

		public static string ScanRunningTaskJobName = $"{DotnetSpiderHubConsts.JobPrefix}ScanRunningTask";
		public static string UpgradeTaskSchedulerTaskName = $"{DotnetSpiderHubConsts.JobPrefix}UpgradeTaskScheduler";

		public SystemAppService(INodeAppService nodeAppService, IMessageAppService messageAppService,
			ISchedulerAppService schedulerAppService,
			ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, userManager)
		{
			_schedulerAppService = schedulerAppService;
			_nodeAppService = nodeAppService;
			_messageAppService = messageAppService;
		}

		public void Register()
		{
			AddScanRunningTaskJob();
			AddUpgradSchedulerJob();
		}

		public void Execute(string name, string arguments)
		{
			if (name == ScanRunningTaskJobName)
			{
				ScanRunningTask();
			}
			else if (name == UpgradeTaskSchedulerTaskName)
			{
				UpgradeScheduler();
			}
		}

		public void UpgradeScheduler()
		{
			foreach (var task in DbContext.Task)
			{
				if (task.Cron == Configuration.IngoreCron)
				{
					_schedulerAppService.Delete(task.Id.ToString());
				}
				else
				{
					var taskId = task.Id.ToString();
					var job = new SchedulerJobDto
					{
						Id = taskId,
						Name = task.Name,
                       // Groupt=task.
						Cron = task.Cron,
						Url = string.Format(Configuration.SchedulerCallback, taskId),
						Data = JsonConvert.SerializeObject(new { TaskId = taskId })
					};
					_schedulerAppService.Create(job);
				}
			}
		}

		private void AddScanRunningTaskJob()
		{
			Core.Entities.Task scanRunningTask = DbContext.Task.FirstOrDefault(t => t.Name == ScanRunningTaskJobName);
			if (scanRunningTask == null)
			{
				scanRunningTask = new Core.Entities.Task
				{
					ApplicationName = "DotnetSpider.Hub",
					Cron = $"0/15 * * * *",
					IsEnabled = true,
					IsDeleted = true,
					Developers = "DotnetSpider",
					Owners = "DotnetSpider",
					Arguments = "",
					NodeCount = 1,
					NodeRunningCount = 0,
					Name = ScanRunningTaskJobName,
					Package = "0001",
					NodeType = "HUB"
				};
				DbContext.Task.Add(scanRunningTask);
				DbContext.SaveChanges();
			}
			var taskId = scanRunningTask.Id.ToString();
			var job = new SchedulerJobDto
			{
				Id = taskId,
				Name = scanRunningTask.Name,
				Cron = "0/15 * * * *",
				Url = string.Format(Configuration.SchedulerCallback, taskId),
				Data = JsonConvert.SerializeObject(new { TaskId = taskId })
			};
			_schedulerAppService.Create(job);
		}

		private void AddUpgradSchedulerJob()
		{
			Core.Entities.Task upgradeScheduler = DbContext.Task.FirstOrDefault(t => t.Name.StartsWith(UpgradeTaskSchedulerTaskName));
			if (upgradeScheduler == null)
			{
				upgradeScheduler = new Core.Entities.Task
				{
					ApplicationName = "DotnetSpider.Hub",
					Cron = "* * * * *",
					IsEnabled = true,
					IsDeleted = true,
					Developers = "DotnetSpider.Hub",
					Owners = "DotnetSpider.Hub",
					Arguments = "",
					NodeCount = 1,
					NodeRunningCount = 0,
					Name = UpgradeTaskSchedulerTaskName,
					Package = "0001",
					NodeType = "HUB"
				};
				DbContext.Task.Add(upgradeScheduler);
				DbContext.SaveChanges();
			}
			var taskId = upgradeScheduler.Id.ToString();
			var job = new SchedulerJobDto
			{
				Id = taskId,
				Name = upgradeScheduler.Name,
				Cron = "* * * * *",
				Url = string.Format(Configuration.SchedulerCallback, taskId),
				Data = JsonConvert.SerializeObject(new { TaskId = taskId })
			};
			_schedulerAppService.Create(job);
		}

		private void ScanRunningTask()
		{
			//var runningTasks = DbContext.Task.Where(t => t.IsRunning).ToList();
			//foreach (var task in runningTasks)
			//{
			//	var id = task.Id;
			//	var status = DbContext.TaskStatus.Where(ts => ts.TaskId == id).OrderByDescending(ts => ts.LastModificationTime).FirstOrDefault();
			//	if (status != null)
			//	{
			//		var time = status.LastModificationTime != null ? status.LastModificationTime.Value : status.CreationTime;

			//		if ((DateTime.Now - time).TotalSeconds > 3600)
			//		{
			//			TaskUtil.ExitTask(_nodeAppService, _messageAppService, task, Logger);
			//		}
			//	}
			//}
			//DbContext.SaveChanges();
		}
	}
}
