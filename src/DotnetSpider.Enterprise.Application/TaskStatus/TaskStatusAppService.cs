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

namespace DotnetSpider.Enterprise.Application.Node
{
	public class TaskStatusAppService : AppServiceBase, ITaskStatusAppService
	{
		public TaskStatusAppService(ApplicationDbContext dbcontext) : base(dbcontext)
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
	}
}
