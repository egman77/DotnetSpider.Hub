using AutoMapper;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Application.NodeHeartbeat.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Domain.Entities;

namespace DotnetSpider.Enterprise.Application
{
	public class AutoMapperConfiguration
	{
		public static void CreateMap()
		{
			Mapper.Initialize((config) =>
			{
				config.CreateMap<CreateTaskInput, Domain.Entities.Task>();
				config.CreateMap<Domain.Entities.Task, CreateTaskInput>();
				config.CreateMap<NodeHeartbeat.Dto.NodeHeartbeatInput, Domain.Entities.NodeHeartbeat>();
				config.CreateMap<MessageHistory, Domain.Entities.Message>();
				config.CreateMap<Domain.Entities.Message, MessageHistory>();
				config.CreateMap<Domain.Entities.Node, NodeDto>();
				config.CreateMap<AddOrUpdateTaskStatusInput, Domain.Entities.TaskStatus>();
				config.CreateMap<Domain.Entities.TaskStatus, TaskStatusDto>();
				config.CreateMap<AddTaskHistoryInput, Domain.Entities.TaskHistory>();
				config.CreateMap<CreateMessageInput, Domain.Entities.Message>();
				config.CreateMap<Domain.Entities.Message, MessageDto>();
				config.CreateMap<Domain.Entities.Task, TaskDto>();
				config.CreateMap<MessageDto, NodeHeartbeatOutput>();
			});
		}
	}
}
