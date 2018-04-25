using System;
using System.Collections.Generic;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using AutoMapper;
using System.Linq;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Application.System;
using DotnetSpider.Enterprise.Application.Scheduler;
using DotnetSpider.Enterprise.Application.Scheduler.Dtos;
using Newtonsoft.Json;
using System.Linq.Expressions;
using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.Application.Task
{
	public class TaskAppService : AppServiceBase, ITaskAppService
	{
		private readonly ITaskHistoryAppService _taskHistoryAppService;
		private readonly IMessageAppService _messageAppService;
		private readonly INodeAppService _nodeAppService;
		private readonly ISystemAppService _systemAppService;
		private readonly ISchedulerAppService _schedulerAppService;

		public TaskAppService(ISchedulerAppService schedulerAppService, ISystemAppService systemAppService,
			ITaskHistoryAppService taskHistoryAppService,
			IMessageAppService messageAppService,
			INodeAppService nodeAppService, ICommonConfiguration configuration, IAppSession appSession, UserManager<ApplicationUser> userManager,
			ApplicationDbContext dbcontext, ILoggerFactory loggerFactory) : base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_schedulerAppService = schedulerAppService;
			_systemAppService = systemAppService;
			_taskHistoryAppService = taskHistoryAppService;
			_messageAppService = messageAppService;
			_nodeAppService = nodeAppService;
		}

		public PaginationQueryDto Find(PaginationQueryInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}
			input.Validate();
			PaginationQueryDto output;

			Expression<Func<Core.Entities.Task, bool>> where = t => !t.IsDeleted;

			var keyword = input.GetFilterValue("keyword");

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				where = where.AndAlso(t => t.Name.Contains(keyword));
			}

			var isRunning = input.GetFilterValue("isrunning");
			if (!string.IsNullOrWhiteSpace(isRunning))
			{
				if ("true" == isRunning.ToLower())
				{
					where = where.AndAlso(t => t.IsRunning);
				}
				else
				{
					where = where.AndAlso(t => !t.IsRunning);
				}
			}

			switch (input.Sort)
			{
				case "name":
					{
						output = DbContext.Task.PageList(input, where, t => t.Name);
						break;
					}
				case "nodecount":
					{
						output = DbContext.Task.PageList(input, where, t => t.NodeCount);
						break;
					}
				default:
					{
						output = DbContext.Task.PageList(input, where, t => t.CreationTime);
						break;
					}
			}
			output.Result = Mapper.Map<List<TaskDto>>(output.Result);
			return output;
		}

		public void Create(CreateTaskInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return;
			}
			var task = Mapper.Map<Core.Entities.Task>(input);

			input.ApplicationName = input.ApplicationName.Trim();
			input.Arguments = input.Arguments?.Trim();
			input.Cron = input.Cron.Trim();
			input.Version = input.Version.Trim();
			input.Name = input.Name.Trim();

			var cron = task.Cron;
			task.Cron = DotnetSpiderConsts.UnTriggerCron;
			DbContext.Task.Add(task);
			DbContext.SaveChanges();

			if (cron != DotnetSpiderConsts.UnTriggerCron)
			{

				var taskId = task.Id.ToString();
				var job = new SchedulerJobDto
				{
					Id = taskId,
					Name = task.Name,
					Cron = cron,
					Url = string.Format(Configuration.SchedulerCallback, taskId),
					Data = JsonConvert.SerializeObject(new { TaskId = taskId })
				};
				_schedulerAppService.Create(job);
				task.Cron = cron;
				DbContext.Task.Update(task);
				DbContext.SaveChanges();
			}
		}

		public void Update(UpdateTaskInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return;
			}
			var task = DbContext.Task.FirstOrDefault(a => a.Id == input.Id);
			if (task == null)
			{
				throw new Exception("Unfound task.");
			}
			task.Analysts = input.Analysts?.Trim();
			task.ApplicationName = input.ApplicationName?.Trim();
			task.Arguments = input.Arguments?.Trim();

			task.Cron = input.Cron;
			task.Description = input.Description?.Trim();
			task.Developers = input.Developers?.Trim();

			task.Name = input.Name?.Trim();
			task.NodeCount = input.NodeCount;
			task.NodeRunningCount = input.NodeRunningCount;
			task.Os = input.Os?.Trim();
			task.Owners = input.Owners?.Trim();
			task.Tags = input.Tags?.Trim();
			task.NodeType = input.NodeType;
			task.Version = input.Version?.Trim();
			task.IsSingle = input.IsSingle;

			if (!input.IsEnabled && task.IsEnabled && task.Cron == DotnetSpiderConsts.UnTriggerCron)
			{
				_schedulerAppService.Delete(task.Id.ToString());
			}

			if (input.IsEnabled)
			{
				var taskId = task.Id.ToString();
				var job = new SchedulerJobDto
				{
					Id = taskId,
					Name = task.Name,
					Cron = task.Cron,
					Url = string.Format(Configuration.SchedulerCallback, taskId),
					Data = JsonConvert.SerializeObject(new { TaskId = taskId })
				};
				_schedulerAppService.Update(job);
			}

			task.IsEnabled = input.IsEnabled;
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
		}

		public void Run(long taskId)
		{
			var msg = DbContext.Message.FirstOrDefault(a => a.TaskId == taskId && Core.Entities.Message.RunMessageName == a.Name);
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
			var runMessage = DbContext.Message.FirstOrDefault(m => m.TaskId == taskId && Core.Entities.Message.RunMessageName == m.Name);
			if (runMessage != null)
			{
				DbContext.Message.Remove(runMessage);
			}

			var cancelMsg = DbContext.Message.FirstOrDefault(a => a.TaskId == task.Id && Core.Entities.Message.CanleMessageName == a.Name);
			if (cancelMsg != null)
			{
				return;
			}

			TaskUtil.ExitTask(_nodeAppService, _messageAppService, task, Logger);

			DbContext.SaveChanges();
		}

		public void Delete(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task != null)
			{
				_schedulerAppService.Delete(task.Id.ToString());
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

			_schedulerAppService.Delete(task.Id.ToString());
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

			var taskIdStr = taskId.ToString();
			var job = new SchedulerJobDto
			{
				Id = taskIdStr,
				Name = task.Name,
				Cron = task.Cron,
				Url = string.Format(Configuration.SchedulerCallback, taskId),
				Data = JsonConvert.SerializeObject(new { TaskId = taskIdStr })
			};
			_schedulerAppService.Create(job);
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
			Logger.LogInformation($"Enable task {taskId}.");
		}

		public void IncreaseRunning(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderException("任务不存在!");
			}
			task.NodeRunningCount += 1;
			DbContext.SaveChanges();
			Logger.LogInformation($"IncreaseRunning task { taskId}.");
		}

		public void ReduceRunning(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
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
			Logger.LogInformation($"ReduceRunning task { taskId}.");
			DbContext.SaveChanges();

		}

		public PaginationQueryDto QueryRunning(PaginationQueryInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}
			PaginationQueryDto output = DbContext.Task.PageList(input, d => d.IsRunning, d => d.Id);
			output.Result = Mapper.Map<List<TaskDto>>(output.Result);
			return output;
		}

		public CreateTaskInput Find(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderException("任务不存在.");
			}

			return Mapper.Map<CreateTaskInput>(task);
		}

		/// <summary>
		/// 判断任务状态
		/// 若任务无效、已删除、或在运行中，则抛出异常
		/// </summary>
		/// <param name="taskId">任务ID</param>
		/// <returns>任务对象</returns>
		private Core.Entities.Task CheckStatusOfTask(long taskId)
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

			var runMessage = DbContext.Message.FirstOrDefault(m => m.TaskId == taskId && m.Name == Core.Entities.Message.RunMessageName);
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

		private string PushTask(Core.Entities.Task task)
		{
			var nodes = _nodeAppService.GetAvailable(task.Os, task.NodeType, task.NodeCount);

			if (nodes.Count == 0)
			{
				// TODO LOG
				return null;
			}

			var identity = Guid.NewGuid().ToString("N");
			var messages = new List<CreateMessageInput>();
			foreach (var node in nodes)
			{
				var arguments = string.Concat(task.Arguments, task.IsSingle ? " -tid:" : " ", task.Id, task.IsSingle ? " -i:" : " ", identity);
				var msg = new CreateMessageInput
				{
					TaskId = task.Id,
					ApplicationName = task.ApplicationName,
					Name = Core.Entities.Message.RunMessageName,
					NodeId = node.NodeId,
					Version = task.Version,
					Arguments = arguments
				};
				messages.Add(msg);
			}
			_messageAppService.Create(messages);

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
					_schedulerAppService.Delete(task.Id.ToString());
				}
				else
				{
					var taskId = task.Id.ToString();
					var job = new SchedulerJobDto
					{
						Id = taskId,
						Name = task.Name,
						Cron = task.Cron,
						Url = string.Format(Configuration.SchedulerCallback, taskId),
						Data = JsonConvert.SerializeObject(new { TaskId = taskId })
					};
					_schedulerAppService.Create(job);
				}
			}
		}

		public void Control(long taskId, ActionType action)
		{
			switch (action)
			{
				case ActionType.Disable:
					{
						Disable(taskId);
						break;
					}
				case ActionType.Enable:
					{
						Enable(taskId);
						break;
					}
				case ActionType.Exit:
					{
						Exit(taskId);
						break;
					}
				case ActionType.Run:
					{
						Run(taskId);
						break;
					}
				case ActionType.Increase:
					{
						IncreaseRunning(taskId);
						break;
					}
				case ActionType.Reduce:
					{
						ReduceRunning(taskId);
						break;
					}
			}
		}
	}
}
