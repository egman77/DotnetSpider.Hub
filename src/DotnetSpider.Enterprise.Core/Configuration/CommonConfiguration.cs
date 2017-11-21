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

		public string MsSqlConnectionString
		{
			get
			{
				var connectionStrings = AppConfiguration.GetSection("ConnectionStrings");
				return connectionStrings.GetValue<string>(DotnetSpiderConsts.ConnectionName);
			}
		}

		public string LogMongoConnectionString
		{
			get
			{
				var logMongoConnectionString = AppConfiguration.GetSection("ConnectionStrings");
				return logMongoConnectionString.GetValue<string>(DotnetSpiderConsts.LogMongoConnectionName);
			}
		}

		public string SmsApi
		{
			get
			{
				var section = AppConfiguration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<string>(DotnetSpiderConsts.SmsApi);
			}
		}

		public string XmlKeyPath
		{
			get
			{
				var section = AppConfiguration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<string>(DotnetSpiderConsts.XmlKeyPath);
			}
		}

		public string SchedulerUrl
		{
			get
			{
				var section = AppConfiguration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<string>(DotnetSpiderConsts.SchedulerUrl);
			}
		}

		public string SchedulerCallbackHost
		{
			get
			{
				var section = AppConfiguration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<string>(DotnetSpiderConsts.SchedulerCallbackHost);
			}
		}

		public string HostUrl { get; set; }
	}
}
