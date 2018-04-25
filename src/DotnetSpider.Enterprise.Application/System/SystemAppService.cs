using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Scheduler;
using DotnetSpider.Enterprise.Application.Scheduler.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.Application.System
{
	public class SystemAppService : AppServiceBase, ISystemAppService
	{
		private readonly ISchedulerAppService _schedulerAppService;
		private readonly IMessageAppService _messageAppService;
		private readonly INodeAppService _nodeAppService;

		public const string ScanRunningTaskName = "System.DotnetSpider.ScanRunningTask";

		public SystemAppService(INodeAppService nodeAppService, IMessageAppService messageAppService, ISchedulerAppService schedulerAppService,
			ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_schedulerAppService = schedulerAppService;
			_nodeAppService = nodeAppService;
			_messageAppService = messageAppService;
		}

		public void Register()
		{
			Core.Entities.Task scanRunningTask = DbContext.Task.FirstOrDefault(t => t.Name.StartsWith(ScanRunningTaskName));
			if (scanRunningTask == null)
			{
				scanRunningTask = new Core.Entities.Task
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
