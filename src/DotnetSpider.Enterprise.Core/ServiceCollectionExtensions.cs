using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotnetSpider.Enterprise.Core
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
