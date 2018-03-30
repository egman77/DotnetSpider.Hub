using Microsoft.Extensions.Configuration;

namespace DotnetSpider.Enterprise.Core.Configuration
{
	public interface ICommonConfiguration
	{
		IConfigurationRoot AppConfiguration { get; set; }
		string MsSqlConnectionString { get; }
		string MySqlConnectionString { get; }
		string SchedulerUrl { get; }
		string SchedulerCallback { get; }
		string HostUrl { get; set; }
		string[] Tokens { get; set; }
		byte[] SqlEncryptKey { get; set; }
		bool AuthorizeApi { get; }
		bool RecordGloabException { get; }
	}
}
