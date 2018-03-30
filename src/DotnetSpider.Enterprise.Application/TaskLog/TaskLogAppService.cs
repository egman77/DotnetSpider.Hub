using AutoMapper;
using DotnetSpider.Enterprise.Application.TaskLog.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DotnetSpider.Enterprise.Application.TaskLog
{
	public class TaskLogAppService : AppServiceBase, ITaskLogAppService
	{
		public TaskLogAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<Domain.Entities.ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
		}

		public void Add(AddTaskLogInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return;
			}

			var taskLog = Mapper.Map<Domain.Entities.TaskLog>(input);
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

			Expression<Func<Domain.Entities.TaskLog, bool>> where = t => t.Identity == identity;

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
