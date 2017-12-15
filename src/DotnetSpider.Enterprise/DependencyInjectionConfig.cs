using DotnetSpider.Enterprise.Application;
using DotnetSpider.Enterprise.Application.AuthMessage;
using DotnetSpider.Enterprise.Application.Hangfire;
using DotnetSpider.Enterprise.Application.Log;
using DotnetSpider.Enterprise.Application.Message;
using DotnetSpider.Enterprise.Application.Node;
using DotnetSpider.Enterprise.Application.Pipeline;
using DotnetSpider.Enterprise.Application.Report;
using DotnetSpider.Enterprise.Application.System;
using DotnetSpider.Enterprise.Application.Task;
using DotnetSpider.Enterprise.Application.TaskHistory;
using DotnetSpider.Enterprise.Application.TaskStatus;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Enterprise
{
	public class DependencyInjectionConfig
	{
		public static void Inject(IServiceCollection services)
		{
			// Add application services.
			services.AddTransient<IEmailSender, AuthMessageAppServices>();
			services.AddTransient<ISmsSender, AuthMessageAppServices>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


			services.AddScoped<IAppSession, AppSession>();

			services.AddSingleton<ICommonConfiguration, CommonConfiguration>();
			services.AddScoped<ILogAppService, LogAppService>();
			services.AddScoped<ITaskAppService, TaskAppService>();
			services.AddScoped<INodeAppService, NodeAppService>();
			services.AddScoped<IMessageAppService, MessageAppService>();
			services.AddScoped<ITaskStatusAppService, TaskStatusAppService>();
			services.AddScoped<ITaskHistoryAppService, TaskHistoryAppService>();
			services.AddScoped<IReportAppService, ReportAppService>();
			services.AddScoped<IPipelineAppService, PipelineAppService>();
			services.AddScoped<IHangfireAppService, HangfireAppService>();
			services.AddScoped<ISystemAppService, SystemAppService>();

		}
	}
}
