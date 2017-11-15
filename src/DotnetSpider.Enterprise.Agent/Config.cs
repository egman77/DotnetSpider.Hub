using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetSpider.Enterprise.Agent
{
	public class Config
	{
		private static IConfigurationRoot _configuration;

		public static string BaseDataDirectory { get; set; }
		public static string RunningLockPath { get; set; }
		public static string NodeIdPath { get; set; }
		public static string ProjectsDirectory { get; set; }
		public static string PackagesDirectory { get; set; }
		public static bool IsRunningOnWindows { get; }
		public static string NodeId { get; set; }
		public static string Ip { get; set; }
		public static string HostName { get; set; }
		public static string Os { get; set; }
		public const string Version = "1.0.0";
		public static string PackageUrl { get; set; }
		public static string ServerUrl { get; set; }
		public static string HeartbeatUrl { get; set; }
		public static int HeartbeatInterval { get; set; }
		public static string ApiToken { get; set; }
		public static double CpuFullLoad { get; set; }

		public static void Load(IConfigurationRoot configuration)
		{
			_configuration = configuration;
		}

		static Config()
		{
			IsRunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

			RunningLockPath = Path.Combine(AppContext.BaseDirectory, "agent.lock");
			NodeIdPath = Path.Combine(AppContext.BaseDirectory, "nodeId");
			ProjectsDirectory = Path.Combine(AppContext.BaseDirectory, "projects");
			PackagesDirectory = Path.Combine(AppContext.BaseDirectory, "packages");
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Os = "Linux";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Os = "Windows";
			}
			else
			{
				Os = "OSX";
			}

			if (!Directory.Exists(ProjectsDirectory))
			{
				Directory.CreateDirectory(ProjectsDirectory);
			}
			if (!Directory.Exists(PackagesDirectory))
			{
				Directory.CreateDirectory(PackagesDirectory);
			}
			HostName = Dns.GetHostName();
			var addressList = Dns.GetHostAddressesAsync(HostName).Result;
			IPAddress localaddr = addressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToList()[0];
			Ip = localaddr.ToString();
			CpuFullLoad = 0.7 * Environment.ProcessorCount;
		}

		public static void Save()
		{
			File.WriteAllText(NodeIdPath, $"{NodeId}{Environment.NewLine}");
		}

		public static void Load()
		{
			var builder = new ConfigurationBuilder();
			builder.AddIniFile("config.ini");

			_configuration = builder.Build();

			PackageUrl = _configuration.GetValue<string>("packageUrl");
			ServerUrl = _configuration.GetValue<string>("serverUrl");
			HeartbeatInterval = _configuration.GetValue<int>("heartbeatInterval");
			HeartbeatUrl = $"{ServerUrl}node/heartbeat";
			ApiToken = _configuration.GetValue<string>("apiToken");

			if (File.Exists(NodeIdPath))
			{
				var lines = File.ReadAllLines(NodeIdPath);
				NodeId = lines.FirstOrDefault();
			}
			if (string.IsNullOrEmpty(NodeId))
			{
				NodeId = Ip;
				Save();
			}
		}
	}
}
