using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Core
{
	public static class ServiceCollectionExtensions
	{
		public static IDotnetSpiderEnterpriseBuilder AddDotnetSpiderEnterprise(this IServiceCollection services, Action<IDotnetSpiderEnterpriseBuilder> config)
		{
			IDotnetSpiderEnterpriseBuilder builder = new DotnetSpiderEnterpriseBuilder(services);
			config(builder);
			return builder;
		}
	}
}
