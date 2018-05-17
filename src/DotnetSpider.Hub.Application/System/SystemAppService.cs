using System.Linq;
using DotnetSpider.Hub.Application.Message;
using DotnetSpider.Hub.Application.Node;
using DotnetSpider.Hub.Application.Scheduler;
using DotnetSpider.Hub.Application.Scheduler.Dtos;
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

		public const string ScanRunningTaskName = "System.DotnetSpider.ScanRunningTask";

		public SystemAppService(INodeAppService nodeAppService, IMessageAppService messageAppService,
			ISchedulerAppService schedulerAppService,
			ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			IAppSession appSession, UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
			_schedulerAppService = schedulerAppService;
			_nodeAppService = nodeAppService;
			_messageAppService = messageAppService;
		}

		public void Register()
		{
			Hub.Core.Entities.Task scanRunningTask = DbContext.Task.FirstOrDefault(t => t.Name.StartsWith(ScanRunningTaskName));
			if (scanRunningTask == null)
			{
				scanRunningTask = new Hub.Core.Entities.Task
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
