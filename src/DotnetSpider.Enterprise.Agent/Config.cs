using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Agent
{
	public class Config
	{
		private static IConfigurationRoot _configuration;

		public static void Load(IConfigurationRoot configuration)
		{
			_configuration = configuration;
		}

		public static Config Instance = new Config();

		private Config()
		{
		}

		public string PackageUrl
		{
			get
			{
				return _configuration.GetValue<string>("packageUrl");
			}
		}

		public string HeartbeatUrl
		{
			get
			{
				return _configuration.GetValue<string>("heartbeatUrl");
			}
		}

		public int HeartbeatInterval
		{
			get
			{
				return _configuration.GetValue<int>("heartbeatInterval");
			}
		}
	}
}
