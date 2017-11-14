using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using AutoMapper;
using System.Linq;
using DotnetSpider.Enterprise.Core;
using System.Data;
using DotnetSpider.Enterprise.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using DotnetSpider.Enterprise.Application.Exceptions;
using System.IO;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using MongoDB.Driver;
using MongoDB.Bson;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;

namespace DotnetSpider.Enterprise.Application.Task
{
	public class TaskAppService : AppServiceBase, ITaskAppService
	{
		private readonly ICommonConfiguration _configuration;
		private readonly ITaskHistoryAppService _taskHistoryAppService;
		private readonly IMessageAppService _messageAppService;
		private readonly INodeAppService _nodeAppService;

		public TaskAppService(ITaskHistoryAppService taskHistoryAppService, IMessageAppService messageAppService, INodeAppService nodeAppService, ICommonConfiguration configuration,
			ApplicationDbContext dbcontext) : base(dbcontext)
		{
			_configuration = configuration;
			_taskHistoryAppService = taskHistoryAppService;
			_messageAppService = messageAppService;
			_nodeAppService = nodeAppService;
		}

		public QueryTaskOutputDto Query(PagingQueryTaskInputDto input)
		{
			input.Validate();

			PagingQueryOutputDto result;
			if (string.IsNullOrWhiteSpace(input.Keyword?.Trim()))
			{
				result = DbContext.Task.PageList(input, null, t => t.CreationTime);
			}
			else
			{
				result = DbContext.Task.PageList(input, t => t.Name.Contains(input.Keyword), t => t.CreationTime);
			}

			QueryTaskOutputDto output = new QueryTaskOutputDto
			{
				Page = result.Page,
				Result = Mapper.Map<List<AddTaskInputDto>>(result.Result),
				Size = result.Size,
				Total = result.Total
			};
			return output;
		}

		public void Add(AddTaskInputDto item)
		{
			var task = Mapper.Map<Domain.Entities.Task>(item);
			// FOR TEST
			task.Cron = "*/2 * * * *";

			item.ApplicationName = item.ApplicationName.Trim();
			item.Arguments = item.Arguments?.Trim();
			item.Cron = item.Cron?.Trim();
			item.Version = item.Version?.Trim();
			item.Name = item.Name?.Trim();

			var cron = task.Cron;
			// DEFAULT VALUE
			task.Cron = "0 0 0 ? 2013-2014";
			DbContext.Task.Add(task);
			DbContext.SaveChanges();

			if (AddOrUpdateHangfireJob(task.Id, cron))
			{
				task.Cron = cron;
				DbContext.Task.Update(task);
				DbContext.SaveChanges();
			}
		}

		public void Modify(ModifyTaskInputDto item)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == item.Id);
			if (task == null)
			{
				throw new Exception("Unfound task.");
			}
			task.Analysts = item.Analysts?.Trim();
			task.ApplicationName = item.ApplicationName?.Trim();
			task.Arguments = item.Arguments?.Trim();
			task.Cron = "*/2 * * * *";
			// TODO
			//task.Cron = item.Cron;
			task.Description = item.Description?.Trim();
			task.Developers = item.Developers?.Trim();

			task.Name = item.Name?.Trim();
			task.NodeCount = item.NodeCount;
			task.NodeRunningCount = item.NodeRunningCount;
			task.Os = item.Os?.Trim();
			task.Owners = item.Owners?.Trim();
			task.Tags = item.Tags?.Trim();
			task.Version = item.Version?.Trim();

			if (!task.IsEnabled && item.IsEnabled)
			{
				AddOrUpdateHangfireJob(task.Id, task.Cron);
			}
			task.IsEnabled = item.IsEnabled;
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
		}

		/// <summary>
		/// 通知Scheduler服务
		/// 只尝试一次，失败抛出异常
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="cron"></param>
		private bool AddOrUpdateHangfireJob(long taskId, string cron)
		{
			var url = $"{_configuration.SchedulerUrl}Task/AddOrUpdate";

			var json = JsonConvert.SerializeObject(new HangfireJobDto
			{
				Name = taskId.ToString(),
				Cron = cron,
				Url = $"{_configuration.HostUrl}Task/Fire",
				Data = taskId.ToString()
			});
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var result = Util.Client.PostAsync(url, content).Result;
				result.EnsureSuccessStatusCode();
				return true;
			}
			catch (Exception ex)
			{
				throw new SchedulerException("调用Scheduler服务异常:" + ex.Message, ex);
			}
		}

		private void RemoveHangfireJob(long taskId)
		{
			var url = $"{_configuration.SchedulerUrl}Task/Remove";
			var postData = $"jobId={taskId}";
			var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
			var result = Util.Client.PostAsync(url, content).Result;
			result.EnsureSuccessStatusCode();
		}

		public void Run(long taskId)
		{
			var msg = DbContext.Message.FirstOrDefault(a => a.TaskId == taskId && a.Name == "RUN");
			if (msg == null)
			{
				var task = CheckStatusOfTask(taskId);
				PushTask(task);
			}
		}

		public void Exit(string identity)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.LastIdentity == identity);
			if (task == null || task.NodeRunningCount <= 0)
			{
				throw new Exception("任务不在运行中.");
			}

			var cancelMsg = DbContext.Message.FirstOrDefault(a => a.TaskId == task.Id && a.Name == "CANCEL");
			if (cancelMsg != null)
			{
				return;
			}

			var taskStatus = DbContext.TaskStatus.Where(a => a.Identity == identity).ToList();
			if (taskStatus == null || taskStatus.Count == 0)
			{
				throw new Exception("当前任务没有上报状态!");
			}

			//判断状态，是否需要考虑时间过期的？？如超过5分钟未上报的，是否为在运行中
			var runningNodes = taskStatus.Where(a => !(a.Status == "Finished" || a.Status == "Exited"));
			foreach (var status in runningNodes)
			{
				var msg = new Domain.Entities.Message
				{
					ApplicationName = string.Empty,
					Arguments = string.Empty,
					TaskId = task.Id,
					Name = "CANCEL",
					NodeId = status.NodeId
				};
				DbContext.Message.Add(msg);
			}
			if (runningNodes.Any())
			{
				DbContext.SaveChanges();
			}
		}

		public void Remove(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task != null)
			{
				RemoveHangfireJob(task.Id);
				DbContext.Task.Remove(task);
				DbContext.SaveChanges();
			}
		}

		public void Disable(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new Exception("任务不存在!");
			}
			task.IsEnabled = false;

			RemoveHangfireJob(task.Id);
			DbContext.Task.Update(task);
			DbContext.SaveChanges();
		}

		public void Enable(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new Exception("任务不存在!");
			}
			task.IsEnabled = true;
			if (AddOrUpdateHangfireJob(task.Id, task.Cron))
			{
				DbContext.Task.Update(task);
				DbContext.SaveChanges();
			}
		}

		public void IncreaseRunning(TaskIdInputDto input)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == input.TaskId);
			if (task == null)
			{
				throw new Exception("任务不存在!");
			}
			task.NodeRunningCount += 1;
			DbContext.SaveChanges();
		}

		public void ReduceRunning(TaskIdInputDto input)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == input.TaskId);
			if (task == null)
			{
				throw new Exception("任务不存在!");
			}
			if (task.NodeRunningCount > 0)
			{
				task.NodeRunningCount -= 1;
			}
			DbContext.SaveChanges();
		}

		public PagingQueryOutputDto QueryRunning(PagingQueryInputDto input)
		{
			PagingQueryOutputDto output = DbContext.Task.PageList(input, d => d.NodeRunningCount > 0, d => d.Id);
			output.Result = Mapper.Map<List<RunningTaskOutputDto>>(output.Result);
			return output;
		}

		public AddTaskInputDto Get(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new Exception("任务不存在.");
			}

			return AutoMapper.Mapper.Map<AddTaskInputDto>(task);
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
			if (task == null || task.IsDeleted)
			{
				throw new AppException("任务不存在");
			}
			if (!task.IsEnabled)
			{
				throw new AppException("任务已被禁用");
			}

			if (task.NodeRunningCount > 0)
			{
				throw new Exception("任务正在运行中");
			}

			if (!string.IsNullOrEmpty(task.LastIdentity))
			{
				var statusList = DbContext.TaskStatus.Where(a => a.Identity == task.LastIdentity);
				if (statusList.Any(a => a.Status != "Exited" && a.Status != "Finished"
					&& a.LastModificationTime.HasValue
					&& (DateTime.Now - a.LastModificationTime).Value.TotalSeconds < 120))
				{
					throw new Exception("任务正在运行中");
				}
			}
			return task;
		}

		private void PushTask(Domain.Entities.Task task)
		{
			var nodes = _nodeAppService.GetAvailableNodes(task.Os, task.NodeCount);

			if (nodes.Count == 0)
			{
				// TODO LOG
				return;
			}

			var identity = Guid.NewGuid().ToString("N");
			var messages = new List<AddMessageInputDto>();
			foreach (var node in nodes)
			{
				var msg = new AddMessageInputDto
				{
					TaskId = task.Id,
					ApplicationName = task.ApplicationName,
					Name = "RUN",
					NodeId = node.NodeId,
					Version = task.Version,
					Arguments = string.Concat(task.Arguments, "-tid:", task.Id, " ", "-i:", identity)
				};
			}
			_messageAppService.AddRange(messages);

			var taskHistory = new AddTaskHistoryInputDto
			{
				Identity = identity,
				NodeIds = string.Join("|", nodes.Select(a => a.NodeId)),
				TaskId = task.Id
			};
			_taskHistoryAppService.Add(taskHistory);
		}
	}
}
