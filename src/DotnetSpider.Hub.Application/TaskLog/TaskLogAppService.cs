using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;
using DotnetSpider.Hub.Application.TaskLog.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Hub.Application.TaskLog
{
	public class TaskLogAppService : AppServiceBase, ITaskLogAppService
	{
		public TaskLogAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, userManager)
		{
		}

		public void Add(params AddTaskLogInput[] input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}

			var taskLog = Mapper.Map<Core.Entities.TaskLog[]>(input);
			DbContext.TaskLog.AddRange(taskLog);
			DbContext.SaveChanges();
		}

		public PaginationQueryDto Find(PaginationQueryTaskLogInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}

			var identity = input.Identity;

			Expression<Func<Core.Entities.TaskLog, bool>> where = t => t.Identity == identity;

			var nodeId = input.NodeId;

			if (!string.IsNullOrWhiteSpace(nodeId))
			{
				where = where.AndAlso(t => t.NodeId == nodeId);
			}

			var logType = input.LogType;
			if (!string.IsNullOrWhiteSpace(logType) && "all" != logType.Trim().ToLower())
			{
				where = where.AndAlso(t => t.Level.ToLower() == logType.ToLower());
			}
			var output = DbContext.TaskLog.PageList(input, where, t => t.Logged);
			output.Result = Mapper.Map<List<TaskLogOutput>>(output.Result);
			return output;
		}
	}
}
