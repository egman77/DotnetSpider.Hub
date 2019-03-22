using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Core
{
	public static class ServiceCollectionExtensions
	{
        /// <summary>
        /// 添加程序构建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
		public static IDotnetSpiderHubBuilder AddDotnetSpiderHub(this IServiceCollection services, Action<IDotnetSpiderHubBuilder> config)
		{
			IDotnetSpiderHubBuilder builder = new DotnetSpiderHubBuilder(services);
			config(builder);
			return builder;
		}
	}
}
