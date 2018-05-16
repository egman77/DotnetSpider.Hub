using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Enterprise.Core
{
	public interface IDotnetSpiderEnterpriseBuilder
	{
		ICommonConfiguration Configuratoin { get; }
		IServiceCollection Services { get; }
		void UseConfiguration(IConfiguration configuration);
	}
}
