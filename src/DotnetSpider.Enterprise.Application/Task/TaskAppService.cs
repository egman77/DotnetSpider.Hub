using System;
using System.Collections.Generic;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using AutoMapper;
using System.Linq;
using DotnetSpider.Enterprise.Core;
using MongoDB.Driver;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Application.System;
using DotnetSpider.Enterprise.Application.Hangfire;
using Newtonsoft.Json;

namespace DotnetSpider.Enterprise.Application.Task
{
	public class TaskAppService : AppServiceBase, ITaskAppService
	{
		private readonly ITaskHistoryAppService _taskHistoryAppService;
		private readonly IMessageAppService _messageAppService;
		private readonly INodeAppService _nodeAppService;
		private readonly ISystemAppService _systemAppService;
		private readonly IHangfireAppService _hangfireAppService;

		public TaskAppService(IHangfireAppService hangfireAppService, ISystemAppService systemAppService,
			ITaskHistoryAppService taskHistoryAppService,
			IMessageAppService messageAppService,
			INodeAppService nodeAppService, ICommonConfiguration configuration, IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager,
			ApplicationDbContext dbcontext, ILoggerFactory loggerFactory) : base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_hangfireAppService = hangfireAppService;
			_systemAppService = systemAppService;
			_taskHistoryAppService = taskHistoryAppService;
			_messageAppService = messageAppService;
			_nodeAppService = nodeAppService;
		}

		public PaginationQueryDto Query(PaginationQueryTaskInput input)
		{
			PaginationQueryDto result;
			if (string.IsNullOrWhiteSpace(input.Keyword?.Trim()))
			{
				result = DbContext.Task.PageList(input, t => !t.IsDeleted, t => t.CreationTime);
			}
			else
			{
				result = DbContext.Task.PageList(input, t => t.Name.Contains(input.Keyword) && !t.IsDeleted, t => t.CreationTime);
			}

			PaginationQueryDto output = new PaginationQueryDto
			{
				Page = result.Page,
				Result = Mapper.Map<List<TaskDto>>(result.Result),
				Size = result.Size,
				Total = result.Total
			};
			return output;
		}

		public void Add(AddTaskInput item)
		{
			var task = Mapper.Map<Domain.Entities.Task>(item);

			item.ApplicationName = item.ApplicationName.Trim();
			item.Arguments = item.Arguments?.Trim();
			item.Cron = item.Cron.Trim();
			item.Version = item.Version.Trim();
			item.Name = item.Name.Trim();

			var cron = task.Cron;
			task.Cron = DotnetSpiderConsts.UnTriggerCron;
			DbContext.Task.Add(task);
			DbContext.SaveChanges();

			if (cron != DotnetSpiderConsts.UnTriggerCron && _hangfireAppService.AddOrUpdateJob(task.Id.ToString(), cron))
			{
				task.Cron = cron;
				DbContext.Task.Update(task);
				DbContext.SaveChanges();
			}
		}

		public void Modify(ModifyTaskInput item)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == item.Id);
			if (task == null)
			{
				throw new Exception("Unfound task.");
			}
			task.Analysts = item.Analysts?.Trim();
			task.ApplicationName = item.ApplicationName?.Trim();
			task.Arguments = item.Arguments?.Trim();

			task.Cron = item.Cron;
			task.Description = item.Description?.Trim();
			task.Developers = item.Developers?.Trim();

			task.Name = item.Name?.Trim();
			task.NodeCount = item.NodeCount;
			task.NodeRunningCount = item.NodeRunningCount;
			task.Os = item.Os?.Trim();
			task.Owners = item.Owners?.Trim();
			task.Tags = item.Tags?.Trim();
			task.NodeType = item.NodeType;
			task.Version = item.Version?.Trim();
			task.IsSingle = item.IsSingle;

			if (task.Cron == DotnetSpiderConsts.UnTriggerCron)
			{
				_hangfireAppService.RemoveJob(task.Id.ToString());
			}
			else
			{
				_hangfireAppService.AddOrUpdateJob(task.Id.ToString(), string.Join(" ", task.Cron));
			}

			task.IsEnabled = item.IsEnabled;
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
		}

		public void Run(long taskId)
		{
			var msg = DbContext.Message.FirstOrDefault(a => a.TaskId == taskId && a.Name == "RUN");
			if (msg == null)
			{
				var task = CheckStatusOfTask(taskId);
				if (task.Name.StartsWith(DotnetSpiderConsts.SystemJobPrefix))
				{
					_systemAppService.Execute(task.Name, task.Arguments);
					Logger.LogWarning($"Run task {taskId}.");
				}
				else
				{
					var identity = PushTask(task);
					if (!string.IsNullOrEmpty(identity))
					{
						task.LastIdentity = identity;
						task.IsRunning = true;
						DbContext.SaveChanges();
						Logger.LogWarning($"Run task {taskId}.");
					}
				}
			}
		}

		public void Exit(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new Exception("Task unfound.");
			}

			// 如果运行的命令还没有被节点消费, 则直接删除运行消息, 减少节点的消耗。
			var runMessage = DbContext.Message.FirstOrDefault(m => m.TaskId == taskId && m.Name == Domain.Entities.Message.RunMessageName);
			if (runMessage != null)
			{
				DbContext.Message.Remove(runMessage);
			}

			var cancelMsg = DbContext.Message.FirstOrDefault(a => a.TaskId == task.Id && a.Name == "CANCEL");
			if (cancelMsg != null)
			{
				return;
			}

			TaskUtil.ExitTask(_nodeAppService, _messageAppService, task, Logger);

			DbContext.SaveChanges();
		}

		public void Remove(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task != null)
			{
				_hangfireAppService.RemoveJob(task.Id.ToString());
				task.IsDeleted = true;
				DbContext.SaveChanges();
				Logger.LogInformation($"Remove task {taskId}.");
			}
		}

		public void Disable(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderException("任务不存在!");
			}
			task.IsEnabled = false;

			_hangfireAppService.RemoveJob(task.Id.ToString());
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
			Logger.LogInformation($"Disable task {taskId}.");
		}

		public void Enable(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderException("任务不存在!");
			}
			task.IsEnabled = true;
			if (_hangfireAppService.AddOrUpdateJob(task.Id.ToString(), task.Cron))
			{
				DbContext.Task.Update(task);
				DbContext.SaveChanges();
				Logger.LogInformation($"Enable task {taskId}.");
			}
		}

		public void IncreaseRunning(TaskIdInput input)
		{
			if (IsAuth())
			{
				var task = DbContext.Task.FirstOrDefault(a => a.Id == input.TaskId);
				if (task == null)
				{
					throw new DotnetSpiderException("任务不存在!");
				}
				task.NodeRunningCount += 1;
				DbContext.SaveChanges();
				Logger.LogInformation($"IncreaseRunning task { input.TaskId}.");
			}
			else
			{
				throw new DotnetSpiderException("Access Denied.");
			}
		}

		public void ReduceRunning(TaskIdInput input)
		{
			if (IsAuth())
			{
				var task = DbContext.Task.FirstOrDefault(a => a.Id == input.TaskId);
				if (task == null)
				{
					throw new DotnetSpiderException("任务不存在!");
				}
				if (task.NodeRunningCount > 0)
				{
					task.NodeRunningCount -= 1;
				}
				if (task.NodeRunningCount == 0)
				{
					task.IsRunning = false;
				}
				Logger.LogInformation($"ReduceRunning task { input.TaskId}.");
				DbContext.SaveChanges();
			}
			else
			{
				throw new DotnetSpiderException("Access Denied.");
			}
		}

		public PaginationQueryDto QueryRunning(PaginationQueryInput input)
		{
			PaginationQueryDto output = DbContext.Task.PageList(input, d => d.IsRunning, d => d.Id);
			output.Result = Mapper.Map<List<TaskDto>>(output.Result);
			return output;
		}

		public AddTaskInput Get(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderException("任务不存在.");
			}

			return Mapper.Map<AddTaskInput>(task);
		}

		/// <summary>
		/// 判断任务状态
		/// 若任务无效、已删除、或在运行中，则抛出异常
		/// </summary>
		/// <param name="taskId">任务ID</param>
		/// <returns>任务对象</returns>
		private Domain.Entities.Task CheckStatusOfTask(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);

			if (task == null)
			{
				throw new DotnetSpiderException("任务不存在");
			}
			if (task.Name.StartsWith(DotnetSpiderConsts.SystemJobPrefix))
			{
				return task;
			}
			if (task.IsDeleted)
			{
				throw new DotnetSpiderException("任务已被删除");
			}
			if (!task.IsEnabled)
			{
				throw new DotnetSpiderException("任务已被禁用");
			}
			if (task.NodeRunningCount > 0)
			{
				throw new DotnetSpiderException("任务正在运行中");
			}

			var runMessage = DbContext.Message.FirstOrDefault(m => m.TaskId == taskId && m.Name == Domain.Entities.Message.RunMessageName);
			if (runMessage != null)
			{
				throw new DotnetSpiderException("已发送运行命令");
			}
			if (!string.IsNullOrEmpty(task.LastIdentity))
			{
				var statusList = DbContext.TaskStatus.Where(a => a.Identity == task.LastIdentity);
				if (statusList.Any(a => a.Status != "Exited" && a.Status != "Finished"
					&& a.LastModificationTime.HasValue
					&& (DateTime.Now - a.LastModificationTime).Value.TotalSeconds < 120))
				{
					throw new DotnetSpiderException("任务正在运行中");
				}
			}
			return task;
		}

		private string PushTask(Domain.Entities.Task task)
		{
			var nodes = _nodeAppService.GetAvailable(task.Os, task.NodeType, task.NodeCount);

			if (nodes.Count == 0)
			{
				// TODO LOG
				return null;
			}

			var identity = Guid.NewGuid().ToString("N");
			var messages = new List<AddMessageInput>();
			foreach (var node in nodes)
			{
				var arguments = string.Concat(task.Arguments, task.IsSingle ? " -tid:" : " ", task.Id, task.IsSingle ? " -i:" : " ", identity);
				var msg = new AddMessageInput
				{
					TaskId = task.Id,
					ApplicationName = task.ApplicationName,
					Name = Domain.Entities.Message.RunMessageName,
					NodeId = node.NodeId,
					Version = task.Version,
					Arguments = arguments
				};
				messages.Add(msg);
			}
			_messageAppService.Add(messages);

			var taskHistory = new AddTaskHistoryInput
			{
				Identity = identity,
				NodeIds = string.Join("|", nodes.Select(a => a.NodeId)),
				TaskId = task.Id
			};
			_taskHistoryAppService.Add(taskHistory);
			return identity;
		}

		public void UpgradeScheduler()
		{
			foreach (var task in DbContext.Task)
			{
				if (task.Cron == DotnetSpiderConsts.UnTriggerCron)
				{
					_hangfireAppService.RemoveJob(task.Id.ToString());
				}
				else
				{
					_hangfireAppService.AddOrUpdateJob(task.Id.ToString(), string.Join(" ", task.Cron));
				}
			}
		}
	}
}
