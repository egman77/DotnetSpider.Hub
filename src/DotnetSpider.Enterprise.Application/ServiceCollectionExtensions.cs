using DotnetSpider.Enterprise.Application.AuthMessage;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.NodeHeartbeat;
using DotnetSpider.Enterprise.Application.Pipeline;
using DotnetSpider.Enterprise.Application.Report;
using DotnetSpider.Enterprise.Application.Scheduler;
using DotnetSpider.Enterprise.Application.System;
using DotnetSpider.Enterprise.Application.Task;
using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Application.TaskLog;
using DotnetSpider.Enterprise.Application.TaskStatus;
using DotnetSpider.Enterprise.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Enterprise.Application
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
