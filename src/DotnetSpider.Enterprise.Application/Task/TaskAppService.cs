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

		public QueryTaskOutputDto GetList(QueryTaskInputDto input)
		{
			input.Init();

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
			var proj = Mapper.Map<Domain.Entities.Task>(item);
			DbContext.Tasks.Add(proj);
			DbContext.SaveChanges();
			item.Id = proj.Id;
		}

		public void ModifyTask(TaskDto item)
		{
			var proj = Mapper.Map<Domain.Entities.Task>(item);
			DbContext.Tasks.Update(proj);
			if (DbContext.SaveChanges() > 0)
			{
				//notify scheduler center
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
