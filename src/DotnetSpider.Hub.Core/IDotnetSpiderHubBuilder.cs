using DotnetSpider.Hub.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Hub.Core
{
	public interface IDotnetSpiderHubBuilder
	{
		ICommonConfiguration Configuratoin { get; }
		IServiceCollection Services { get; }
		void UseConfiguration(IConfiguration configuration);
	}
}
