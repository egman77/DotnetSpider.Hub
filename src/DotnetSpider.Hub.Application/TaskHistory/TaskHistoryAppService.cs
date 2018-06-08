using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DotnetSpider.Hub.Application.TaskHistory.Dtos;
using DotnetSpider.Hub.Application.TaskStatus.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Hub.Application.TaskHistory
{
	public class TaskHistoryAppService : AppServiceBase, ITaskHistoryAppService
	{
		public TaskHistoryAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, userManager)
		{
		}

		public void Add(AddTaskHistoryInput input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}
			var taskHistory = Mapper.Map<Core.Entities.TaskHistory>(input);
			DbContext.TaskHistory.Add(taskHistory);
			DbContext.SaveChanges();
		}

		public PaginationQueryDto Find(PaginationQueryTaskHistoryInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}
			var output = new PaginationQueryDto
			{
				Page = input.Page.Value,
				Size = input.Size.Value
			};
			var taskId = input.TaskId;
			var taskHistoryOutput = DbContext.TaskHistory.PageList(input, a => a.TaskId == taskId, t => t.CreationTime);

			output.Total = taskHistoryOutput.Total;
			var taskHistories =(List<Core.Entities.TaskHistory>) taskHistoryOutput.Result;
			List<Core.Entities.TaskStatus> statuses;
			if (taskHistories.Count > 0)
			{
				var identities = taskHistories.Select(r => r.Identity);
				statuses = DbContext.TaskStatus.Where(a => identities.Contains(a.Identity)).ToList();
			}
			else
			{
				statuses = new List<Core.Entities.TaskStatus>(0);
			}

			var result = new List<TaskHistoryOutput>(taskHistories.Count);
			var statusOutputs = Mapper.Map<List<TaskStatusDto>>(statuses);

			foreach (var item in taskHistories)
			{
				result.Add(new TaskHistoryOutput
				{
					Identity = item.Identity,
					CreationTime = item.CreationTime.ToString("yyyy/MM/dd HH:mm:ss"),
					Statuses = statusOutputs.Where(a => a.Identity == item.Identity).ToList()
				});
			}
			output.Result = result;

			return output;

		}
	}
}
