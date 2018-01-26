using AutoMapper;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetSpider.Enterprise.Application.TaskHistory
{
	public class TaskHistoryAppService : AppServiceBase, ITaskHistoryAppService
	{
		public TaskHistoryAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<Domain.Entities.ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
		}

		public void Add(AddTaskHistoryInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return;
			}
			var taskHistory = Mapper.Map<Domain.Entities.TaskHistory>(input);
			DbContext.TaskHistory.Add(taskHistory);
			DbContext.SaveChanges();
		}

		public PaginationQueryDto Find(PaginationQueryInput input)
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
			var taskId = long.Parse(input.GetFilterValue("taskid"));
			var taskHistoryOutput = DbContext.TaskHistory.PageList(input, a => a.TaskId == taskId, t => t.CreationTime);

			output.Total = taskHistoryOutput.Total;
			var taskHistories =(List<Domain.Entities.TaskHistory>) taskHistoryOutput.Result;
			List<Domain.Entities.TaskStatus> statuses;
			if (taskHistories.Count > 0)
			{
				var identities = taskHistories.Select(r => r.Identity);
				statuses = DbContext.TaskStatus.Where(a => identities.Contains(a.Identity)).ToList();
			}
			else
			{
				statuses = new List<Domain.Entities.TaskStatus>(0);
			}

			var result = new List<PaginationQueryTaskHistoryDto>(taskHistories.Count);
			var statusOutputs = Mapper.Map<List<TaskStatusDto>>(statuses);

			foreach (var item in taskHistories)
			{
				result.Add(new PaginationQueryTaskHistoryDto
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
