using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Core
{
	public static class ServiceCollectionExtensions
	{
		public static IDotnetSpiderHubBuilder AddDotnetSpiderHub(this IServiceCollection services, Action<IDotnetSpiderHubBuilder> config)
		{
			IDotnetSpiderHubBuilder builder = new DotnetSpiderHubBuilder(services);
			config(builder);
			return builder;
		}
	}
}
