using AutoMapper;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Application.NodeHeartbeat.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Application.TaskLog.Dto;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.Application
{
	internal class AutoMapperConfiguration
	{
		public static void CreateMap()
		{
			Mapper.Initialize((config) =>
			{
				config.CreateMap<CreateTaskInput, Core.Entities.Task>();
				config.CreateMap<Core.Entities.Task, CreateTaskInput>();
				config.CreateMap<NodeHeartbeatInput, Core.Entities.NodeHeartbeat>();
				config.CreateMap<MessageHistory, Core.Entities.Message>();
				config.CreateMap<Core.Entities.Message, MessageHistory>();
				config.CreateMap<Core.Entities.Node, NodeDto>();
				config.CreateMap<AddOrUpdateTaskStatusInput, Core.Entities.TaskStatus>();
				config.CreateMap<Core.Entities.TaskStatus, TaskStatusDto>();
				config.CreateMap<AddTaskHistoryInput, Core.Entities.TaskHistory>();
				config.CreateMap<CreateMessageInput, Core.Entities.Message>();
				config.CreateMap<Core.Entities.Message, MessageDto>();
				config.CreateMap<Core.Entities.Task, TaskDto>();
				config.CreateMap<MessageDto, NodeHeartbeatOutput>();
				config.CreateMap<AddTaskLogInput, Core.Entities.TaskLog>();
				config.CreateMap<Core.Entities.TaskLog, TaskLogOutput>();
				
			});
		}
	}
}
