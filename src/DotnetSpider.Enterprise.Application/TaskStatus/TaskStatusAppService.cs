using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Newtonsoft.Json;
using DotnetSpider.Enterprise.Domain.Entities;
using AutoMapper;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Message.Dto;
using DotnetSpider.Enterprise.Application.TaskStatus;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace DotnetSpider.Enterprise.Application.Node
{
	public class TaskStatusAppService : AppServiceBase, ITaskStatusAppService
	{
		public TaskStatusAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<Domain.Entities.ApplicationUser> userManager)
			: base(dbcontext, configuration, appSession, userManager)
		{
		}

		public void AddOrUpdate(AddOrUpdateTaskStatusInputDto input)
		{
			var oldRecord = DbContext.TaskStatus.FirstOrDefault(ts => ts.Identity == input.Identity && ts.NodeId == input.NodeId);
			if (oldRecord == null)
			{
				var taskStatus = Mapper.Map<Domain.Entities.TaskStatus>(input);
				DbContext.TaskStatus.Add(taskStatus);
			}
			else
			{
				oldRecord.AvgDownloadSpeed = input.AvgDownloadSpeed;
				oldRecord.AvgPipelineSpeed = input.AvgPipelineSpeed;
				oldRecord.AvgProcessorSpeed = input.AvgProcessorSpeed;
				oldRecord.Error = input.Error;
				oldRecord.Left = input.Left;
				oldRecord.Status = input.Status;
				oldRecord.Success = input.Success;
				oldRecord.Thread = input.Thread;
				oldRecord.Total = input.Total;
			}
			DbContext.SaveChanges();
		}

		public PagingQueryOutputDto Query(PagingQueryTaskStatusInputDto input)
		{
			input.Validate();

			PagingQueryOutputDto output;
			Expression<Func<Domain.Entities.TaskStatus, bool>> where = null;
			var status = input.Status?.ToLower().Trim();
			List<Domain.Entities.Task> tasks;
			List<long> taskIds;
			List<Domain.Entities.TaskStatus> taskStatuses;
			if (string.IsNullOrWhiteSpace(input.Keyword) && string.IsNullOrEmpty(input.Keyword))
			{
				if (!string.IsNullOrEmpty(status) && "all" != status)
				{
					where = d => d.Status.ToLower() == status;
				}
				output = DbContext.TaskStatus.PageList(input, where, d => d.Id);
				taskStatuses = output.Result as List<Domain.Entities.TaskStatus>;
				taskIds = taskStatuses.Select(t => t.TaskId).ToList();
				tasks = DbContext.Task.Where(t => taskIds.Contains(t.Id)).ToList();
			}
			else
			{
				tasks = DbContext.Task.Where(t => t.Name.ToLower().Contains(input.Keyword.ToLower())).ToList();
				taskIds = tasks.Select(t => t.Id).ToList();
				if (!string.IsNullOrEmpty(status) && "all" != status)
				{
					where = d => d.Status.ToLower() == status && taskIds.Contains(d.TaskId);
				}
				else
				{
					where = d => taskIds.Contains(d.TaskId);
				}
				output = DbContext.TaskStatus.PageList(input, where, d => d.Id);
				taskStatuses = output.Result as List<Domain.Entities.TaskStatus>;
			}
			var taskStatusOutputs = new List<TaskStatusOutputDto>();

			taskIds = taskStatuses.Select(t => t.TaskId).ToList();
			foreach (var taskStatus in taskStatuses)
			{
				var taskStatusOutput = new TaskStatusOutputDto();
				taskStatusOutput.Name = tasks.FirstOrDefault(t => t.Id == taskStatus.TaskId)?.Name;
				taskStatusOutput.AvgDownloadSpeed = taskStatus.AvgDownloadSpeed;
				taskStatusOutput.AvgPipelineSpeed = taskStatus.AvgDownloadSpeed;
				taskStatusOutput.AvgProcessorSpeed = taskStatus.AvgDownloadSpeed;
				taskStatusOutput.Error = taskStatus.Error;
				taskStatusOutput.Identity = taskStatus.Identity;
				taskStatusOutput.LastModificationTime = taskStatus.LastModificationTime?.ToString("yyyy/MM/dd HH:mm:ss");
				taskStatusOutput.Left = taskStatus.Left;
				taskStatusOutput.NodeId = taskStatus.NodeId;
				taskStatusOutput.Status = taskStatus.Status;
				taskStatusOutput.Success = taskStatus.Success;
				taskStatusOutput.TaskId = taskStatus.TaskId;
				taskStatusOutput.Thread = taskStatus.Thread;
				taskStatusOutput.Total = taskStatus.Total;
				taskStatusOutputs.Add(taskStatusOutput);
			}
			output.Result = taskStatusOutputs;
			return output;
		}
	}
}
