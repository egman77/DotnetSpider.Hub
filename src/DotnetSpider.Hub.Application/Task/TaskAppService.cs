using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using DotnetSpider.Hub.Application.Message;
using DotnetSpider.Hub.Application.Message.Dtos;
using DotnetSpider.Hub.Application.Node;
using DotnetSpider.Hub.Application.Scheduler;
using DotnetSpider.Hub.Application.Scheduler.Dtos;
using DotnetSpider.Hub.Application.System;
using DotnetSpider.Hub.Application.Task.Dtos;
using DotnetSpider.Hub.Application.TaskHistory;
using DotnetSpider.Hub.Application.TaskHistory.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DotnetSpider.Hub.Application.Task
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
			INodeAppService nodeAppService, ICommonConfiguration configuration, UserManager<ApplicationUser> userManager,
			ApplicationDbContext dbcontext) : base(dbcontext, configuration, userManager)
		{
			_schedulerAppService = schedulerAppService;
			_systemAppService = systemAppService;
			_taskHistoryAppService = taskHistoryAppService;
			_messageAppService = messageAppService;
			_nodeAppService = nodeAppService;
		}

		public PaginationQueryDto Query(PaginationQueryTaskInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}
			input.Validate();
			PaginationQueryDto output;

			Expression<Func<Core.Entities.Task, bool>> where = t => !t.IsDeleted;

			var keyword = input.Keyword;

			if (!string.IsNullOrWhiteSpace(keyword))
			{
				where = where.AndAlso(t => t.Name.Contains(keyword));
			}

			var isRunning = input.IsRunning;
			if (isRunning.HasValue)
			{
				if (isRunning.Value)
				{
					where = where.AndAlso(t => t.IsRunning);
				}
				else
				{
					where = where.AndAlso(t => !t.IsRunning);
				}
			}

			switch (input.Sort?.ToLower())
			{
				case "name":
					{
						output = DbContext.Task.PageList<Core.Entities.Task, string, string>(input, where, t => t.Name);
						break;
					}
				case "nodecount":
					{
						output = DbContext.Task.PageList<Core.Entities.Task, string, int>(input, where, t => t.NodeCount);
						break;
					}
				default:
					{
						output = DbContext.Task.PageList<Core.Entities.Task, string, DateTime>(input, where, t => t.CreationTime);
						break;
					}
			}
			output.Result = Mapper.Map<List<TaskDto>>(output.Result);
			return output;
		}

        /// <summary>
        /// 添加一个任务
        /// </summary>
        /// <param name="input"></param>
		public void Create(CreateTaskInput input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}
            //映射到任务对象
			var task = Mapper.Map<Core.Entities.Task>(input);

			input.ApplicationName = input.ApplicationName.Trim();
			input.Arguments = input.Arguments?.Trim();
			input.Cron = input.Cron.Trim();
			input.Package = input.Package.Trim();
			input.Name = input.Name.Trim();

            //表示要创建议计划?
			if (input.Cron != Configuration.IngoreCron)
			{
                //计划工作
				var job = new SchedulerJobDto
				{
					Id = task.Id,
					Name = task.Name,
					Cron = input.Cron,
					Url = string.Format(Configuration.SchedulerCallback, task.Id), //设置回调
					Content = JsonConvert.SerializeObject(new { TaskId = task.Id })
				};

                if (!task.Name.StartsWith(job.Group))
                {
                    task.Name = $"[{job.Group}]{task.Name}";
                }

                _schedulerAppService.Create(job); //创建计划
			}

			DbContext.Task.Add(task); //添加任务
			DbContext.SaveChanges();
		}

		public void Update(UpdateTaskInput input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
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
			task.Package = input.Package?.Trim();
			task.IsSingle = input.IsSingle;

			if (!input.IsEnabled && task.IsEnabled && task.Cron == Configuration.IngoreCron)
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
					Content = JsonConvert.SerializeObject(new { TaskId = taskId })
				};

                if(!task.Name.StartsWith(job.Group))
                {
                    task.Name = $"[{job.Group}]{task.Name}";
                }

				_schedulerAppService.Update(job);
			}

			task.IsEnabled = input.IsEnabled;
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
		}

		public void Run(string taskId)
		{
			var msg = DbContext.Message.FirstOrDefault(a => a.TaskId == taskId && Core.Entities.Message.RunMessageName == a.Name);
			if (msg == null)
			{
				var task = CheckStatusOfTask(taskId);
               
                if (task.Name.StartsWith(DotnetSpiderHubConsts.JobPrefix))
                {
					_systemAppService.Execute(task.Name, task.Arguments);
					Logger.Warning($"Run task {taskId}.");
				}
				else
				{
					var identity = PushTask(task);
					if (!string.IsNullOrEmpty(identity))
					{
						task.LastIdentity = identity;
						task.IsRunning = true;
						DbContext.SaveChanges();
						Logger.Warning($"Run task {taskId}.");
					}
				}
			}
		}

		public void Exit(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new Exception($"Task {taskId} unfound.");
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

			ExitTask(task);

			DbContext.SaveChanges();
		}

		public void Delete(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task != null)
			{
				_schedulerAppService.Delete(task.Id.ToString());
				task.IsDeleted = true;
				DbContext.SaveChanges();
				Logger.Information($"Remove task {taskId}.");
			}
		}

		public void Disable(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderHubException("任务不存在!");
			}
			task.IsEnabled = false;

			_schedulerAppService.Delete(task.Id.ToString());
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
			Logger.Information($"Disable task {taskId}.");
		}

		public void Enable(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderHubException("任务不存在!");
			}
			task.IsEnabled = true;

			var taskIdStr = taskId.ToString();
			var job = new SchedulerJobDto
			{
				Id = taskIdStr,
				Name = task.Name,
				Cron = task.Cron,
				Url = string.Format(Configuration.SchedulerCallback, taskId),
				Content = JsonConvert.SerializeObject(new { TaskId = taskIdStr })
			};
			_schedulerAppService.Create(job);
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
			Logger.Information($"Enable task {taskId}.");
		}

		public void IncreaseRunning(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderHubException("任务不存在!");
			}
			task.NodeRunningCount += 1;
			DbContext.SaveChanges();
			Logger.Information($"IncreaseRunning task { taskId}.");
		}

		public void ReduceRunning(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new DotnetSpiderHubException("任务不存在!");
			}
			if (task.NodeRunningCount > 0)
			{
				task.NodeRunningCount -= 1;
			}
			if (task.NodeRunningCount == 0)
			{
				task.IsRunning = false;
			}
			Logger.Information($"ReduceRunning task { taskId}.");
			DbContext.SaveChanges();
		}

		public PaginationQueryDto QueryRunning(PaginationQueryInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}
			PaginationQueryDto output = DbContext.Task.PageList<Core.Entities.Task, string, string>(input, d => d.IsRunning, d => d.Id);
			output.Result = Mapper.Map<List<TaskDto>>(output.Result);
			return output;
		}

		public TaskDto GetTask(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(t => t.Id == taskId);
			if (task != null)
			{
				return Mapper.Map<TaskDto>(task);
			}
			return null;
		}

		/// <summary>
		/// 判断任务状态
		/// 若任务无效、已删除、或在运行中，则抛出异常
		/// </summary>
		/// <param name="taskId">任务ID</param>
		/// <returns>任务对象</returns>
		private Core.Entities.Task CheckStatusOfTask(string taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);

			if (task == null)
			{
				throw new DotnetSpiderHubException("任务不存在");
			}
			if (task.Name.StartsWith(DotnetSpiderHubConsts.JobPrefix))
			{
				return task;
			}
			if (task.IsDeleted)
			{
				throw new DotnetSpiderHubException("任务已被删除");
			}
			if (!task.IsEnabled)
			{
				throw new DotnetSpiderHubException("任务已被禁用");
			}
			if (task.NodeRunningCount > 0)
			{
				throw new DotnetSpiderHubException("任务正在运行中");
			}

			var runMessage = DbContext.Message.FirstOrDefault(m => m.TaskId == taskId && m.Name == Core.Entities.Message.RunMessageName);
			if (runMessage != null)
			{
				throw new DotnetSpiderHubException("已发送运行命令");
			}
			if (!string.IsNullOrEmpty(task.LastIdentity))
			{
				var statusList = DbContext.TaskStatus.Where(a => a.Identity == task.LastIdentity);
				if (statusList.Any(a => a.Status != "Exited" && a.Status != "Finished"
					&& a.LastModificationTime.HasValue
					&& (DateTime.Now - a.LastModificationTime).Value.TotalSeconds < 120))
				{
					throw new DotnetSpiderHubException("任务正在运行中");
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
				var arguments = string.Concat(task.Arguments, task.IsSingle ? " --tid:" : " ", task.Id, task.IsSingle ? " -i:" : " ", identity);
				var msg = new CreateMessageInput
				{
					TaskId = task.Id,
					ApplicationName = task.ApplicationName,
					Name = Core.Entities.Message.RunMessageName,
					NodeId = node.NodeId,
					Package = task.Package,
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

		private void ExitTask(Core.Entities.Task task)
		{
			var runningNodes = _nodeAppService.GetAllOnline();

			var messages = new List<CreateMessageInput>();
			foreach (var status in runningNodes)
			{
				var msg = new CreateMessageInput
				{
					ApplicationName = "NULL",
					TaskId = task.Id,
					Name = Core.Entities.Message.CanleMessageName,
					NodeId = status.NodeId
				};
				Logger.Warning($"Add CANCLE message: {JsonConvert.SerializeObject(msg)}.");
				messages.Add(msg);
			}
			_messageAppService.Create(messages);

			task.IsRunning = false;
			task.NodeRunningCount = 0;
		}

		public void Control(string taskId, ActionType action)
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
