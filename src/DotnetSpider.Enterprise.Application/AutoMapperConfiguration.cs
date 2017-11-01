using AutoMapper;
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
				//config.CreateMap<TaskDto, Domain.Entities.Task>();
				//config.CreateMap<ProjectDto, Domain.Entities.Project>();
				//config.CreateMap<Domain.Entities.Project, ProjectDto>();
				//config.CreateMap<Domain.Entities.Task, TaskDto>();

				//config.CreateMap<BuildLog, BuildLogDto>();
				//config.CreateMap<NodeStatus, NodeStatusDto>();
				//config.CreateMap<TaskLog, TaskLogDto>();
				//config.CreateMap<ExecuteLog, ExecuteLogDto>();

				//config.CreateMap<ApplicationUser, UserDto>().ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime.ToString("yyyy-MM-dd HH:mm"))); ;
				//config.CreateMap<Department, DepartmentDto>()
				//.ForMember(dest => dest.ExpireDate, opt => opt.MapFrom(src => src.ExpireDate.ToString("yyyy-MM-dd")))
				//.ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime.ToString("yyyy-MM-dd")));

				//config.CreateMap<TaskOnSale, TaskOnSaleDto>().ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime.ToString("yyyy-MM-dd HH:mm"))); ;
			});
		}
	}
}
