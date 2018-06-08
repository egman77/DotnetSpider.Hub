using System;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Core
{
	public class DotnetSpiderHubBuilder : IDotnetSpiderHubBuilder
	{
		public DotnetSpiderHubBuilder(IServiceCollection services)
		{
			Services = services;
		}

		public IServiceCollection Services { get; }

		public ICommonConfiguration Configuratoin { get; private set; }

		public void UseConfiguration(IConfiguration configuration)
		{
			var config = configuration.GetSection(DotnetSpiderHubConsts.Settings).Get<CommonConfiguration>();
			config.ConnectionString = configuration.GetConnectionString(DotnetSpiderHubConsts.DotnetSpiderHub);
			config.SchedulerCallback = new Uri(new Uri(configuration["Urls"]), "api/task/{0}?action=run").ToString();
			Services.AddSingleton<ICommonConfiguration>(config);
		}
	}
}
