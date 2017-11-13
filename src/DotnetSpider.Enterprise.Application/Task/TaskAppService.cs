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

namespace DotnetSpider.Enterprise.Application.Task
{
	public class TaskAppService : AppServiceBase, ITaskAppService
	{
		private readonly ICommonConfiguration _configuration;

		public TaskAppService(ICommonConfiguration configuration,
			ApplicationDbContext dbcontext) : base(dbcontext)
		{
			_configuration = configuration;
		}

		private void DispatchTaskToNodes(Domain.Entities.Task task)
		{
			var nodeList = new Dictionary<string, int>();
			List<Domain.Entities.Node> nodes = null;
			if (task.Os == "All")
			{
				nodes = DbContext.Node.Where(a => a.IsEnable && a.IsOnline).ToList();
			}
			else
			{
				nodes = DbContext.Node.Where(a => a.IsEnable && a.IsOnline && a.Os == task.Os).ToList();
			}

			if (nodes.Count == 0)
			{
				throw new Exception("没有可运行的节点服务器.");
			}

			foreach (var node in nodes)
			{
				var status = DbContext.NodeHeartbeat.Where(a => a.NodeId == node.NodeId).OrderByDescending(a => a.CreationTime).FirstOrDefault();
				var score = 0;
				if ((DateTime.Now - status.CreationTime).TotalSeconds < 120)
				{
					if (status.ProcessCount < 1)
					{
						score = 5;
					}
					else if (status.ProcessCount == 1)
					{
						score = 2;
					}
					else
					{
						score = 0;
					}

					if (status.FreeMemory >= 800)
					{
						score += 3;
					}
					else if (status.FreeMemory >= 500)
					{
						score += 1;
					}
					if (status.CPULoad < 30)
					{
						score += 2;
					}
					else if (status.CPULoad < 50)
					{
						score += 1;
					}
					nodeList.Add(node.NodeId, score);
				}
			}

			var list = nodeList.OrderByDescending(a => a.Value);
			var identity = Guid.NewGuid().ToString("N");
			DbContext.DoWithTransaction(() =>
			{
				foreach (var item in list.Take(task.NodeCount))
				{
					var msg = new DotnetSpider.Enterprise.Domain.Entities.Message
					{
						TaskId = task.Id,
						ApplicationName = task.ApplicationName,
						Name = "RUN",
						NodeId = item.Key,
						Arguments = string.Concat(task.Arguments, "-i:", identity)
					};
					DbContext.Message.Add(msg);
				}
				DbContext.TaskHistory.Add(new TaskHistory
				{
					Identity = identity,
					NodeIds = string.Join("|", list.Take(task.NodeCount).Select(a => a.Key)),
					TaskId = task.Id
				});
			});
		}

		public bool Fire(long taskId)
		{
			RunTask(taskId);
			return true;
		}

		public QueryTaskOutputDto GetList(PagingQueryTaskInputDto input)
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
				Result = Mapper.Map<List<TaskDto>>(result.Result),
				Size = result.Size,
				Total = result.Total
			};
			return output;
		}

		public void AddTask(TaskDto item)
		{
			var task = Mapper.Map<Domain.Entities.Task>(item);
			DbContext.Task.Add(task);
			DbContext.SaveChanges();

			if (!string.IsNullOrEmpty(task.Cron))
			{
				NotifyScheduler(task.Id, task.Cron);
			}
			item.Id = task.Id;
		}

		public void ModifyTask(TaskDto item)
		{
			var taskObj = Mapper.Map<Domain.Entities.Task>(item);
			var task = DbContext.Task.FirstOrDefault(a => a.Id == item.Id);
			if (task == null) throw new Exception("当前任务不存在.");

			//通知Scheduler
			NotifyScheduler(task.Id, task.Cron);
			DbContext.Task.Update(taskObj);
			DbContext.SaveChanges();
		}

		/// <summary>
		/// 通知Scheduler服务
		/// 只尝试一次，失败抛出异常
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="cron"></param>
		private bool NotifyScheduler(long taskId, string cron)
		{
			var client = new HttpClient(new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
				UseCookies = false
			});

			var message = new HttpRequestMessage(HttpMethod.Post, _configuration.SchedulerUrl);
			message.Headers.Add("Cache-Control", "max-age=0");
			message.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			message.Headers.Add("Upgrade-Insecure-Requests", "1");
			message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");

			var requestObject = new SchedulerRequestObject
			{
				Id = taskId,
				Cron = cron,
				Url = _configuration.SchedulerUrl,
				Data = ""
			};

			var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestObject));
			message.Content = new StreamContent(new MemoryStream(data));
			try
			{
				var content = client.SendAsync(message).Result.Content.ReadAsStringAsync().Result;
				var msg = JsonConvert.DeserializeObject(content) as JObject;
				var status = msg.GetValue("status").ToString();
				if (status != "Ok")
				{
					throw new Exception(msg.GetValue("message").ToString());
				}
				return true;
			}
			catch (Exception ex)
			{
				throw new SchedulerException("调用Scheduler服务异常:" + ex.Message, ex);
			}
		}

		public void RunTask(long taskId)
		{
			var msg = DbContext.Message.FirstOrDefault(a => a.TaskId == taskId && a.Name == "RUN");
			if (msg == null)
			{
				var task = ValidateTaskRunningState(taskId);
				DispatchTaskToNodes(task);
			}
		}

		public void StopTask(string identity)
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

		public void RemoveTask(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task != null)
			{
				NotifyScheduler(task.Id, string.Empty);
				DbContext.Task.Remove(task);
				DbContext.SaveChanges();
			}
		}

		public bool Disable(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new Exception("任务不存在!");
			}
			task.IsEnabled = false;
			if (NotifyScheduler(task.Id, string.Empty))
			{
				DbContext.Task.Update(task);
				DbContext.SaveChanges();
			}

			return true;
		}

		public bool Enable(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a => a.Id == taskId);
			if (task == null)
			{
				throw new Exception("任务不存在!");
			}
			task.IsEnabled = true;
			if (NotifyScheduler(task.Id, task.Cron))
			{
				DbContext.Task.Update(task);
				DbContext.SaveChanges();
			}

			return true;
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

		public PagingQueryOutputDto Running(PagingQueryInputDto input)
		{
			PagingQueryOutputDto output = DbContext.Task.PageList(input, d => d.NodeRunningCount > 0, d => d.Id);
			output.Result = Mapper.Map<List<RunningTaskOutputDto>>(output.Result);
			return output;
		}

		/// <summary>
		/// 判断任务状态
		/// 若任务无效、已删除、或在运行中，则抛出异常
		/// </summary>
		/// <param name="taskId">任务ID</param>
		/// <returns>任务对象</returns>
		private Domain.Entities.Task ValidateTaskRunningState(long taskId)
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

		public PagingQueryOutputDto QueryRunHistory(PagingQueryTaskHistoryInputDto input)
		{
			input.Validate();

			var output = new PagingQueryOutputDto
			{
				Page = input.Page,
				Size = input.Size
			};

			var result =  DbContext.TaskHistory.PageList(input, a => a.TaskId == input.TaskId, t => t.CreationTime);

			output.Total = result.Total;
			var results = result.Result as List<TaskHistory>;
			List<Domain.Entities.TaskStatus> status = null;
			if (results.Count > 0)
			{
				var codeList = new string[results.Count];
				for (var index = 0; index < results.Count; index++)
				{
					codeList[index] = results[index].Identity;
				}
				status = DbContext.TaskStatus.Where(a => codeList.Contains(a.Identity)).ToList();
			}
			else
			{
				status = new List<Domain.Entities.TaskStatus>(0);
			}

			var batchesDto = new List<TaskHistoryDto>(results.Count);
			var statusList = Mapper.Map<List<TaskStatusDto>>(status);

			foreach (var item in results)
			{
				batchesDto.Add(new TaskHistoryDto
				{
					Identity = item.Identity,
					TaskId = input.TaskId,
					StatusList = statusList.Where(a => a.Identity == item.Identity).ToList()
				});
			}
			output.Result = batchesDto;

			return output;

		}

		public TaskDto GetTask(long taskId)
		{
			var task = DbContext.Task.FirstOrDefault(a=>a.Id == taskId);
			if (task == null)
			{
				throw new Exception("任务不存在.");
			}

			return AutoMapper.Mapper.Map<TaskDto>(task);
		}
	}
}
