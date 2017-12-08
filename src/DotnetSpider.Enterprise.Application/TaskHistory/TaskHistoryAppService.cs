using AutoMapper;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskHistory
{
	public class TaskHistoryAppService : AppServiceBase, ITaskHistoryAppService
	{
		public TaskHistoryAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
		}

		public void Add(AddTaskHistoryInputDto input)
		{
			var taskHistory = Mapper.Map<Domain.Entities.TaskHistory>(input);
			DbContext.TaskHistory.Add(taskHistory);
			DbContext.SaveChanges();
		}

		public PagingQueryOutputDto Query(PagingQueryTaskHistoryInputDto input)
		{
			input.Validate();

			var output = new PagingQueryOutputDto
			{
				Page = input.Page,
				Size = input.Size
			};

			var taskHistoryOutput = DbContext.TaskHistory.PageList(input, a => a.TaskId == input.TaskId, t => t.CreationTime);

			output.Total = taskHistoryOutput.Total;
			var taskHistories = taskHistoryOutput.Result as List<Domain.Entities.TaskHistory>;
			List<Domain.Entities.TaskStatus> statuses = null;
			if (taskHistories.Count > 0)
			{
				var identities = taskHistories.Select(r => r.Identity);
				statuses = DbContext.TaskStatus.Where(a => identities.Contains(a.Identity)).ToList();
			}
			else
			{
				statuses = new List<Domain.Entities.TaskStatus>(0);
			}

			var result = new List<TaskHistoryOutputDto>(taskHistories.Count);
			var statusOutputs = Mapper.Map<List<TaskStatusOutputDto>>(statuses);

			foreach (var item in taskHistories)
			{
				result.Add(new TaskHistoryOutputDto
				{
					Identity = item.Identity,
					TaskId = input.TaskId,
					CreationTime = item.CreationTime.ToString("yyyy/MM/dd HH:mm:ss"),
					Statuses = statusOutputs.Where(a => a.Identity == item.Identity).ToList()
				});
			}
			output.Result = result;

			return output;

		}
	}
}
