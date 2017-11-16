using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DotnetSpider.Enterprise.Core.Configuration
{
	public class CommonConfiguration : ICommonConfiguration
	{
		public IConfigurationRoot AppConfiguration { get; set; }

		public CommonConfiguration()
		{
			if (File.Exists(Path.Combine(AppContext.BaseDirectory, "host.config")))
			{
				HostUrl = File.ReadAllLines("host.config")[0];
			}
		}

		public int PageMaxSize
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<int>(ConfigurationConsts.PageMaxSize);
			}
		}

		public int PageSize
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<int>(ConfigurationConsts.PageSize);
			}
		}

		public string RedisHost
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.RedisHost);
			}
		}

		public string RedisPassword
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.RedisPassword);
			}
		}

		public int RedisPort
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<int>(ConfigurationConsts.RedisPort);
			}
		}

		public int PageMaxNumber
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<int>(ConfigurationConsts.PageMaxNumber);
			}
		}

		public string MsSqlConnectionString
		{
			get
			{
				var connectionStrings = AppConfiguration.GetSection("ConnectionStrings");
				return connectionStrings.GetValue<string>(ConfigurationConsts.ConnectionName);
			}
		}

		public string LogMongoConnectionString
		{
			get
			{
				var logMongoConnectionString = AppConfiguration.GetSection("ConnectionStrings");
				return logMongoConnectionString.GetValue<string>(ConfigurationConsts.LogMongoConnectionName);
			}
		}

		public string MySqlConnectionString
		{
			get
			{
				var connectionStrings = AppConfiguration.GetSection("ConnectionStrings");
				return connectionStrings.GetValue<string>(ConfigurationConsts.MySqlConnectionName);
			}
		}

		public string EmailSenderHost
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.EmailSenderHost);
			}
		}

		public int EmailSenderPort
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<int>(ConfigurationConsts.EmailSenderPort);
			}
		}

		public string EmailSenderPassword
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.EmailSenderPassword);
			}
		}

		public bool EmailSenderEnableSsl
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<bool>(ConfigurationConsts.EmailSenderEnableSsl);
			}
		}

		public string EmailSenderFromAddress
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.EmailSenderFromAddress);
			}
		}

		public string EmailSenderFromDisplayName
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.EmailSenderFromDisplayName);
			}
		}

		public bool SendQuartzErrorMail
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<bool>(ConfigurationConsts.SendQuartzErrorMail);
			}
		}

		public int SchedulerMinInterval
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<int>(ConfigurationConsts.SchedulerMinInterval);
			}
		}

		public string EnvironmentName
		{
			get
			{
				return AppConfiguration.GetValue<string>(ConfigurationConsts.EnvName);
			}
		}

		public string HostName
		{
			get
			{
				var envName = EnvironmentName;
				string hostName;
				switch (envName)
				{
					case (ConfigurationConsts.DevEnvName):
						{
							hostName = "http://localhost:8009";
							break;
						}
					case (ConfigurationConsts.TestEnvName):
						{
							hostName = "http://localhost:8009";
							break;
						}
					case (ConfigurationConsts.ProductEnvName):
						{
							hostName = "http://localhost:8009";
							break;
						}
					default:
						{
							hostName = "http://localhost:8009";
							break;
						}
				}
				return hostName;
			}
		}

		public int RedisDb
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<int>(ConfigurationConsts.RedisDb);
			}
		}

		public string SmsApi
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.SmsApi);
			}
		}

		public string XmlKeyPath
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.XmlKeyPath);
			}
		}

		public string RedisNamespace
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.RedisNamespace);
			}
		}

		public string SchedulerUrl
		{
			get
			{
				var section = AppConfiguration.GetSection(ConfigurationConsts.DefaultSetting);
				return section.GetValue<string>(ConfigurationConsts.SchedulerUrl);
			}
		}

		public string HostUrl { get; set; }
	}
}
