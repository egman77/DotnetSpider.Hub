using DotnetSpider.Enterprise.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Enterprise.Application.TaskRunning.Dtos;
using AutoMapper;
using System.Linq;

namespace DotnetSpider.Enterprise.Application.TaskRunning
{
	public class TaskRunningAppService : AppServiceBase, ITaskRunningAppService
	{
		public TaskRunningAppService(ApplicationDbContext dbcontext) : base(dbcontext)
		{
		}

		public void Add(AddTaskRunningInputDto input)
		{
			var taskRunning = Mapper.Map<Domain.Entities.TaskRunning>(input);
			DbContext.TaskRunning.Add(taskRunning);
			DbContext.SaveChanges();
		}

		public void Remove(RemoveTaskRunningInputDto input)
		{
			var taskRunning = DbContext.TaskRunning.FirstOrDefault(t => t.TaskId == input.TaskId);
			if (taskRunning != null)
			{
				DbContext.TaskRunning.Remove(taskRunning);
				DbContext.SaveChanges();
			}
		}
	}
}
