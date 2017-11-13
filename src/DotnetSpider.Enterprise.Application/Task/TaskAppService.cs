using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Application.Project;
using AutoMapper;
using System.Linq;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Application.Project.Dtos;
using System.Data;
using DotnetSpider.Enterprise.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using DotnetSpider.Enterprise.Application.Exceptions;
using System.IO;

namespace DotnetSpider.Enterprise.Application.Task
{
	public class TaskAppService : AppServiceBase, ITaskAppService
	{
		private readonly ICommonConfiguration _configuration;
		private readonly IProjectAppService _projectAppService;

		public TaskAppService(ICommonConfiguration configuration, IProjectAppService projectAppService,
			ApplicationDbContext dbcontext) : base(dbcontext)
		{
			_configuration = configuration;
			_projectAppService = projectAppService;
		}

		public void ProcessCountChanged(long taskId, bool isStart)
		{
			var task = DbContext.Tasks.FirstOrDefault(a => a.Id == taskId);
			if (task != null)
			{
				if (isStart)
				{
					task.NodeRunningCount += 1;
				}
				else
				{
					task.NodeRunningCount -= 1;
				}
				task.NodeRunningCount = task.NodeRunningCount < 0 ? 0 : task.NodeRunningCount;
				DbContext.SaveChanges();
			}
		}

		public bool Fire(long taskId)
		{
			var task = DbContext.Tasks.FirstOrDefault(a => a.Id == taskId && a.IsDelete == false && a.IsEnabled == true);
			if (task == null) throw new Exception("任务不存在");

			//判断任务是否在运行中
			if (task.NodeRunningCount > 0)
			{
				var taskStatus = DbContext.TaskStatus.OrderByDescending(a => a.CreationTime).FirstOrDefault();
				if ((taskStatus.Status != "Finished" && taskStatus.Status != "Exited") && (DateTime.Now - taskStatus.CreationTime).TotalSeconds < 120)
				{
					throw new Exception("任务正在运行中");
				}
			}

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
						Name = task.Name,
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
			return true;
		}

		public QueryTaskOutputDto GetList(PagingQueryTaskInputDto input)
		{
			input.Validate();

			PagingQueryOutputDto result;
			if (string.IsNullOrWhiteSpace(input.Keyword?.Trim()))
			{
				result = DbContext.Tasks.PageList(input, t => t.ProjectId == input.SolutionId, t => t.CreationTime);
			}
			else
			{
				result = DbContext.Tasks.PageList(input, t => t.ProjectId == input.SolutionId && t.Name.Contains(input.Keyword), t => t.CreationTime);
			}
			var projects = _projectAppService.GetAll();
			QueryTaskOutputDto output = new QueryTaskOutputDto
			{
				Page = result.Page,
				Result = Mapper.Map<List<TaskDto>>(result.Result),
				Size = result.Size,
				Total = result.Total,
				Projects = projects
			};
			return output;
		}

		public void AddTask(TaskDto item)
		{
			var task = Mapper.Map<Domain.Entities.Task>(item);
			DbContext.Tasks.Add(task);
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
			var task = DbContext.Tasks.FirstOrDefault(a=>a.Id == item.Id);
			if (task == null) throw new Exception("当前任务不存在.");

			//通知Scheduler
			NotifyScheduler(task.Id, task.Cron);
			DbContext.Tasks.Update(taskObj);
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

			var data = Encoding.UTF8.GetBytes($"id={taskId}&cron={cron}");
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

		public List<RunningTaskDto> GetRunningTasks()
		{
			throw new NotSupportedException();
			//List<RunningTasks> runningTasks = null;
			//using (IDbConnection conn = new MySqlConnection(Configuration.MySqlConnectionString))
			//{
			//	runningTasks = conn.Query<RunningTasks>("select * from `task_running`").ToList();
			//}

			//var taskIdList = new List<long>();
			//runningTasks.ForEach(t =>
			//{
			//	taskIdList.Add(t.TaskId);
			//});

			//var taskList = new List<RunningTaskDto>();
			//var tasks = DbContext.Tasks.Where(a => taskIdList.Contains(a.Id)).ToList();
			//foreach (var task in tasks)
			//{
			//	var runTaskItem = runningTasks.First(a => a.TaskId == task.Id);
			//	taskList.Add(new RunningTaskDto
			//	{
			//		Arguments = task.Arguments,
			//		CDate = runTaskItem.CDate,
			//		CountOfNodes = task.CountOfNodes,
			//		CreationTime = task.CreationTime,
			//		Cron = task.Cron,
			//		Id = task.Id,
			//		IsEnabled = task.IsEnabled,
			//		Name = task.Name,
			//		ProjectId = task.ProjectId,
			//		SpiderName = task.SpiderName,
			//		Version = task.Version,
			//		Identity = runTaskItem.Identity
			//	});
			//}

			//return taskList.OrderBy(a => a.CDate).ToList();
		}

		public void RunTask(long taskId)
		{
			throw new NotSupportedException();
			//var task = DbContext.Tasks.FirstOrDefault(a => a.Id == taskId);
			//if (task == null)
			//{
			//	throw new AppException("Task is not exist");
			//}
			//if (!task.IsEnabled)
			//{
			//	throw new AppException("Task is disabled");
			//}
			//task.Project = DbContext.Projects.First(a => a.Id == task.ProjectId);
			//if (!task.Project.IsEnabled)
			//{
			//	throw new AppException("This project is disabled");
			//}
			//var identity = Encrypt.Md5Encrypt($"{task.SpiderName}{DateTime.Now}");
			//var cmd = new Command
			//{
			//	Id = Guid.NewGuid().ToString("N"),
			//	Name = Command.Run,
			//	Data = JsonConvert.SerializeObject(new RunArgument
			//	{
			//		Entry = task.Project.IntervalPath,
			//		Identity = identity,
			//		ExecuteArguments = $"-s:{task.SpiderName} -i:{identity} -a:{task.Arguments} -tid:{task.Id}",
			//		ProjectName = task.Project.Name,
			//		SolutionId = task.ProjectId,
			//		SpiderName = task.SpiderName,
			//		Version = task.Version,
			//		TaskId = task.Id,
			//		FrameworkVersion = task.Project.Framework,
			//		NodeCount = task.CountOfNodes
			//	})
			//};

			//Subscriber.Publish("DOTNETSPIDER_SCHEDULER", JsonConvert.SerializeObject(cmd));
		}

		public List<long> IsTaskRunning(long[] tasks)
		{
			throw new NotSupportedException();
			//using (IDbConnection conn = new MySqlConnection(Configuration.MySqlConnectionString))
			//{
			//	return conn.Query<long>("select taskId from `task_running` where taskId in @Tasks", new { Tasks = tasks }).ToList();
			//}
		}

		public bool TaskRunning(string identity)
		{
			throw new NotSupportedException();
			//using (IDbConnection conn = new MySqlConnection(Configuration.MySqlConnectionString))
			//{
			//	var item = conn.ExecuteScalar<string>("select `identity` from `task_running` where identity=@identity", new { identity = identity });
			//	return item == identity;
			//}
		}


		public void StopTask(string identity)
		{
			//using (IDbConnection conn = new MySqlConnection(Configuration.MySqlConnectionString))
			//{
			//	conn.Execute("delete from `task_running` where `identity`=@Identity", new { Identity = identity });
			//}
			//Subscriber.Publish(identity, "EXIT");
		}

		public void PauseTask(string identity)
		{
			//Subscriber.Publish(identity, "PAUSE");
		}

		public void ResumeTask(string identity)
		{
			//Subscriber.Publish(identity, "CONTINUE");
		}

		public void RemoveTask(long taskId)
		{
			var task = DbContext.Tasks.FirstOrDefault(a => a.Id == taskId);
			if (task != null)
			{
				DbContext.Tasks.Remove(task);
				DbContext.SaveChanges();
			}
		}
	}
}
