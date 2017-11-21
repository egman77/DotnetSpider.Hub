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

namespace DotnetSpider.Enterprise.Application.Node
{
	public class TaskStatusAppService : AppServiceBase, ITaskStatusAppService
	{
		public TaskStatusAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration) : base(dbcontext, configuration)
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

			switch (input.Status?.ToLower())
			{
				case "finished":
					{
						output = DbContext.TaskStatus.PageList(input, d => d.Status == "Finished", d => d.Id);
						break;
					}
				case "init":
					{
						output = DbContext.TaskStatus.PageList(input, d => d.Status == "Init", d => d.Id);
						break;
					}
				case "running":
					{
						output = DbContext.TaskStatus.PageList(input, d => d.Status == "Running", d => d.Id);
						break;
					}
				case "exited":
					{
						output = DbContext.TaskStatus.PageList(input, d => d.Status == "Exited", d => d.Id);
						break;
					}
				case "stopped":
					{
						output = DbContext.TaskStatus.PageList(input, d => d.Status == "Stopped", d => d.Id);
						break;
					}
				case "all":
					{
						output = DbContext.TaskStatus.PageList(input, null, d => d.Id);
						break;
					}
				default:
					{
						output = DbContext.TaskStatus.PageList(input, null, d => d.Id);
						break;
					}
			}
			var taskStatuses = output.Result as List<Domain.Entities.TaskStatus>;
			var taskIds = taskStatuses.Select(t => t.TaskId).ToList();
			var tasks = DbContext.Task.Where(t => taskIds.Contains(t.Id)).ToList();
			var taskStatusOutputs = new List<TaskStatusOutputDto>();

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
