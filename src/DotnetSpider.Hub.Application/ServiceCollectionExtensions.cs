using DotnetSpider.Hub.Application.Message;
using DotnetSpider.Hub.Application.Node;
using DotnetSpider.Hub.Application.NodeHeartbeat;
using DotnetSpider.Hub.Application.Pipeline;
using DotnetSpider.Hub.Application.Report;
using DotnetSpider.Hub.Application.Scheduler;
using DotnetSpider.Hub.Application.System;
using DotnetSpider.Hub.Application.Task;
using DotnetSpider.Hub.Application.TaskHistory;
using DotnetSpider.Hub.Application.TaskLog;
using DotnetSpider.Hub.Application.TaskStatus;
using DotnetSpider.Hub.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Application
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 注册应用服务所需组件
		/// </summary>
		/// <param name="services"></param>
		/// <param name="builderAction"></param>
		public static void UseDotnetSpiderHubServices(this IDotnetSpiderHubBuilder builder)
		{
			AutoMapperConfiguration.CreateMap();

			builder.Services.AddScoped<ITaskLogAppService, TaskLogAppService>();
			builder.Services.AddScoped<ITaskAppService, TaskAppService>();
			builder.Services.AddScoped<INodeAppService, NodeAppService>();
			builder.Services.AddScoped<IMessageAppService, MessageAppService>();
			builder.Services.AddScoped<ITaskStatusAppService, TaskStatusAppService>();
			builder.Services.AddScoped<ITaskHistoryAppService, TaskHistoryAppService>();
			builder.Services.AddScoped<IDashboardAppService, DashboardAppService>();
			builder.Services.AddScoped<IPipelineAppService, PipelineAppService>();
			builder.Services.AddScoped<ISchedulerAppService, SchedulerAppService>();
			builder.Services.AddScoped<ISystemAppService, SystemAppService>();
			builder.Services.AddScoped<INodeHeartbeatAppService, NodeHeartbeatAppService>();
			builder.Services.AddScoped<ISeedData, SeedData>();
		}

		public static void UseSeeData(this IApplicationBuilder app)
		{
			var seedata = app.ApplicationServices.GetRequiredService<ISeedData>();
			seedata.Init();
		}
	}
}
