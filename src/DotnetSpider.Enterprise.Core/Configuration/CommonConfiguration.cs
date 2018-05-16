using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace DotnetSpider.Enterprise.Core.Configuration
{
	public class CommonConfiguration : ICommonConfiguration
	{
		public IConfiguration Configuration { get; private set; }

		public CommonConfiguration(IConfiguration configuration)
		{
			Configuration = configuration;

			if (File.Exists(Path.Combine(AppContext.BaseDirectory, "domain")))
			{
				HostUrl = File.ReadAllLines("domain")[0];
			}

			Tokens = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting).GetValue<string>(DotnetSpiderConsts.Tokens).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t)).ToArray();

			var code = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting).GetValue<string>(DotnetSpiderConsts.SqlEncryptCode).Trim();
			SqlEncryptKey = Encoding.ASCII.GetBytes(code);
		}

		public string MsSqlConnectionString
		{
			get
			{
				var connectionStrings = Configuration.GetSection("ConnectionStrings");
				return connectionStrings.GetValue<string>(DotnetSpiderConsts.ConnectionName);
			}
		}

		public string MySqlConnectionString
		{
			get
			{
				var connectionStrings = Configuration.GetSection("ConnectionStrings");
				return connectionStrings.GetValue<string>(DotnetSpiderConsts.MySqlConnectionName);
			}
		}

		public string SchedulerUrl
		{
			get
			{
				var section = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<string>(DotnetSpiderConsts.SchedulerUrl);
			}
		}

		public string SchedulerCallback
		{
			get
			{
				var section = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<string>(DotnetSpiderConsts.SchedulerCallback);
			}
		}

		public string HostUrl { get; set; }

		public byte[] SqlEncryptKey { get; set; }

		public string[] Tokens { get; set; }

		public bool AuthorizeApi
		{
			get
			{
				var section = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<bool>(DotnetSpiderConsts.AuthorizeApi);
			}
		}

		public bool RecordGloabException
		{
			get
			{
				var section = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting);
				return section.GetValue<bool>(DotnetSpiderConsts.AuthorizeApi);
			}
		}
	}
}
