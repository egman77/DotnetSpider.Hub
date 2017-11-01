using Microsoft.Extensions.Configuration;

namespace DotnetSpider.Enterprise.Core.Configuration
{
	public interface ICommonConfiguration
	{
		IConfigurationRoot AppConfiguration { get; set; }

		string MsSqlConnectionString { get; }
		string RedisHost { get; }
		int RedisPort { get; }
		string RedisPassword { get; }
		int RedisDb { get; }

		string RedisNamespace { get; }

		int PageSize { get; }
		int PageMaxNumber { get; }
		int PageMaxSize { get; }

		string EmailSenderHost { get; }
		int EmailSenderPort { get; }
		string EmailSenderPassword { get; }
		bool EmailSenderEnableSsl { get; }
		string EmailSenderFromAddress { get; }
		string EmailSenderFromDisplayName { get; }

		string SmsApi { get; }
		string XmlKeyPath { get; }
	}
}
