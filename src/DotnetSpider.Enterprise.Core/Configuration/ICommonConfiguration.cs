using Microsoft.Extensions.Configuration;

namespace DotnetSpider.Enterprise.Core.Configuration
{
	public interface ICommonConfiguration
	{
		IConfiguration Configuration { get; }
		string MsSqlConnectionString { get; }
		string MySqlConnectionString { get; }
		string SchedulerUrl { get; }
		string SchedulerCallback { get; }
		string HostUrl { get; }
		string[] Tokens { get; }
		byte[] SqlEncryptKey { get; }
		bool AuthorizeApi { get; }
		bool RecordGloabException { get; }
	}
}
