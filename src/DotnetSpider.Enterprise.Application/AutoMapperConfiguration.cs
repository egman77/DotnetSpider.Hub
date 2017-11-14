using AutoMapper;
using DotnetSpider.Enterprise.Application.Message.Dto;
using DotnetSpider.Enterprise.Application.Message.Dtos;
using DotnetSpider.Enterprise.Application.Node.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;

namespace DotnetSpider.Enterprise.Application
{
	public class AutoMapperConfiguration
	{
		public static void CreateMap()
		{
			Mapper.Initialize((config) =>
			{
				config.CreateMap<TaskDto, Domain.Entities.Task>();
				config.CreateMap<Domain.Entities.Task, TaskDto>();
				config.CreateMap<NodeHeartbeatInputDto, NodeHeartbeat>();
				config.CreateMap<MessageHistory, Domain.Entities.Message>();
				config.CreateMap<Domain.Entities.Message, MessageHistory>();
				config.CreateMap<Domain.Entities.Node, NodeOutputDto>();
				config.CreateMap<AddOrUpdateTaskStatusInputDto, Domain.Entities.TaskStatus>();
				config.CreateMap<Domain.Entities.Task, RunningTaskOutputDto>();
				config.CreateMap<Domain.Entities.TaskStatus, TaskStatusOutputDto>();
				config.CreateMap<AddTaskHistoryInputDto, Domain.Entities.TaskHistory>();
				config.CreateMap<AddMessageInputDto, Domain.Entities.Message>();
				config.CreateMap<Domain.Entities.Message, MessageOutputDto>();
			});
		}
	}
}
