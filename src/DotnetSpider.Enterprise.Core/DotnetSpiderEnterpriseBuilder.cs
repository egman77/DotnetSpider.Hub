using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Enterprise.Core
{
	public class DotnetSpiderEnterpriseBuilder : IDotnetSpiderEnterpriseBuilder
	{
		public DotnetSpiderEnterpriseBuilder(IServiceCollection services)
		{
			Services = services;
		}

		public IServiceCollection Services { get; }

		public ICommonConfiguration Configuratoin { get; private set; }

		public void UseConfiguration(IConfiguration configuration)
		{
			Configuratoin = new CommonConfiguration(configuration);
			Services.AddSingleton(Configuratoin);
		}
	}
}
