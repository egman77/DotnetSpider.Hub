using AutoMapper;
using DotnetSpider.Hub.Application.Message.Dtos;
using DotnetSpider.Hub.Application.Node.Dtos;
using DotnetSpider.Hub.Application.NodeHeartbeat.Dtos;
using DotnetSpider.Hub.Application.Task.Dtos;
using DotnetSpider.Hub.Application.TaskHistory.Dtos;
using DotnetSpider.Hub.Application.TaskLog.Dtos;
using DotnetSpider.Hub.Application.TaskStatus.Dtos;
using DotnetSpider.Hub.Core.Entities;

namespace DotnetSpider.Hub.Application
{
	internal class AutoMapperConfiguration
	{
		public static void CreateMap()
		{
			Mapper.Initialize((config) =>
			{
				config.CreateMap<CreateTaskInput, Hub.Core.Entities.Task>();
				config.CreateMap<Hub.Core.Entities.Task, CreateTaskInput>();
				config.CreateMap<NodeHeartbeatInput, Hub.Core.Entities.NodeHeartbeat>();
				config.CreateMap<MessageHistory, Hub.Core.Entities.Message>();
				config.CreateMap<Hub.Core.Entities.Message, MessageHistory>();
				config.CreateMap<Hub.Core.Entities.Node, NodeDto>();
				config.CreateMap<AddOrUpdateTaskStatusInput, Hub.Core.Entities.TaskStatus>();
				config.CreateMap<Hub.Core.Entities.TaskStatus, TaskStatusDto>();
				config.CreateMap<AddTaskHistoryInput, Hub.Core.Entities.TaskHistory>();
				config.CreateMap<CreateMessageInput, Hub.Core.Entities.Message>();
				config.CreateMap<Hub.Core.Entities.Message, MessageDto>();
				config.CreateMap<Hub.Core.Entities.Task, TaskDto>();
				config.CreateMap<MessageDto, NodeHeartbeatOutput>();
				config.CreateMap<AddTaskLogInput, Hub.Core.Entities.TaskLog>();
				config.CreateMap<Hub.Core.Entities.TaskLog, TaskLogOutput>();
				
			});
		}
	}
}
