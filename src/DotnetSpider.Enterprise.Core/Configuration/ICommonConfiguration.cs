using Microsoft.Extensions.Configuration;

namespace DotnetSpider.Enterprise.Core.Configuration
{
	public interface ICommonConfiguration
	{
		IConfigurationRoot AppConfiguration { get; set; }

		string LogMongoConnectionString { get; }
		string MsSqlConnectionString { get; }
		string MySqlConnectionString { get; }
		string SchedulerUrl { get; }
		string SchedulerCallbackHost { get; }
		string HostUrl { get; set; }
		string[] Tokens { get; set; }
		byte[] SqlEncryptKey { get; set; }
		bool AuthorizeApi { get; }
	}
}
