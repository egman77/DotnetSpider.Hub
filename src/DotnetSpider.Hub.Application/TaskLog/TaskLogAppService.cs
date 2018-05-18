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
		public TaskLogAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
		}

		public void Add(AddTaskLogInput input)
		{
			if (input == null)
			{
				Logger.Error($"{nameof(input)} should not be null.");
				return;
			}

			var taskLog = Mapper.Map<Core.Entities.TaskLog>(input);
			DbContext.TaskLog.Add(taskLog);
			DbContext.SaveChanges();
		}

		public PaginationQueryDto Find(PaginationQueryInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}

			var identity = input.GetFilterValue("identity")?.Trim();
			if (string.IsNullOrWhiteSpace(identity))
			{
				return new PaginationQueryDto { Page = input.Page.Value, Size = input.Size.Value, Total = 0, Result = null };
			}

			Expression<Func<Core.Entities.TaskLog, bool>> where = t => t.Identity == identity;

			var nodeId = input.GetFilterValue("nodeid")?.Trim();

			if (!string.IsNullOrWhiteSpace(nodeId))
			{
				where = where.AndAlso(t => t.NodeId == nodeId);
			}

			var logType = input.GetFilterValue("logtype");
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
