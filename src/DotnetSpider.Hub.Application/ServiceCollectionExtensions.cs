using DotnetSpider.Hub.Application.AuthMessage;
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
		public static void UserDotnetSpiderEnterpriseServices(this IDotnetSpiderEnterpriseBuilder builder)
		{
			AutoMapperConfiguration.CreateMap();

			builder.Services.AddTransient<IEmailSender, AuthMessageAppServices>();
			builder.Services.AddTransient<ISmsSender, AuthMessageAppServices>();

			builder.Services.AddScoped<IAppSession, AppSession>();
			builder.Services.AddScoped<ITaskLogAppService, TaskLogAppService>();
			builder.Services.AddScoped<ITaskAppService, TaskAppService>();
			builder.Services.AddScoped<INodeAppService, NodeAppService>();
			builder.Services.AddScoped<IMessageAppService, MessageAppService>();
			builder.Services.AddScoped<ITaskStatusAppService, TaskStatusAppService>();
			builder.Services.AddScoped<ITaskHistoryAppService, TaskHistoryAppService>();
			builder.Services.AddScoped<IReportAppService, ReportAppService>();
			builder.Services.AddScoped<IPipelineAppService, PipelineAppService>();
			builder.Services.AddScoped<ISchedulerAppService, SchedulerAppService>();
			builder.Services.AddScoped<ISystemAppService, SystemAppService>();
			builder.Services.AddScoped<INodeHeartbeatAppService, NodeHeartbeatAppService>();
		}
	}
}
