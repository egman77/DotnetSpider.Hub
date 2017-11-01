using DotnetSpider.Enterprise.Application;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Enterprise.Web
{
	public class DependencyInjectionConfig
	{
		public static void Inject(IServiceCollection services)
		{
			// Add application services.
			//services.AddTransient<IEmailSender, AuthMessageAppServices>();
			//services.AddTransient<ISmsSender, AuthMessageAppServices>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


			services.AddScoped<IAppSession, AppSession>();

			//services.AddScoped<IPa1PaSession, Pa1PaSession>();
			services.AddSingleton<ICommonConfiguration, CommonConfiguration>();
			//services.AddScoped<IApplicationLogAppService, ApplicationLogAppService>();
			//services.AddScoped<IProjectAppService, ProjectAppService>();
			//services.AddScoped<ITaskAppService, TaskAppService>();
			//services.AddScoped<INodeAppService, NodeAppService>();
		}
	}
}
