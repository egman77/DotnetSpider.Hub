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
            //创建映射关系
			AutoMapperConfiguration.CreateMap();


			builder.Services.AddScoped<ITaskLogAppService, TaskLogAppService>(); //任务日志应用服务
			builder.Services.AddScoped<ITaskAppService, TaskAppService>();//任务应用服务
			builder.Services.AddScoped<INodeAppService, NodeAppService>();//节点应用服务
			builder.Services.AddScoped<IMessageAppService, MessageAppService>();//消息应用服务
			builder.Services.AddScoped<ITaskStatusAppService, TaskStatusAppService>();//任务状态应用服务
			builder.Services.AddScoped<ITaskHistoryAppService, TaskHistoryAppService>();//任务历史应用服务
			builder.Services.AddScoped<IDashboardAppService, DashboardAppService>();//故事板应用服务
			builder.Services.AddScoped<IPipelineAppService, PipelineAppService>();//管道应用服务
			builder.Services.AddScoped<ISchedulerAppService, SchedulerAppService>();//计划应用服务
			builder.Services.AddScoped<ISystemAppService, SystemAppService>();//系统应用服务
			builder.Services.AddScoped<INodeHeartbeatAppService, NodeHeartbeatAppService>();//节点心跳应用服务
			builder.Services.AddScoped<ISeedData, SeedData>();//初始化数据服务
		}

        /// <summary>
        /// 扩展方法
        /// </summary>
        /// <param name="app"></param>
		public static void UseSeeData(this IApplicationBuilder app)
		{
			var seedata = app.ApplicationServices.GetRequiredService<ISeedData>();
			seedata.Init();
		}
	}
}
