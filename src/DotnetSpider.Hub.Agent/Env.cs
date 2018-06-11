using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace DotnetSpider.Hub.Agent
{
	public class Env
	{
		private static IConfigurationRoot _configuration;

		public static string BaseDataDirectory { get; set; }
		public static string RunningLockPath { get; set; }
		public static string NodeIdPath { get; set; }
		public static string ProjectsDirectory { get; set; }
		public static string PackagesDirectory { get; set; }
		public static string ProcessesDirectory { get; set; }
		public static bool IsRunningOnWindows { get; }
		public static string NodeId { get; set; }
		public static string Ip { get; set; }
		public static string HostName { get; set; }
		public static string Os { get; set; }
		public const string Version = "1.0.0";
		public static string ServerUrl { get; set; }
		public static string HeartbeatUrl { get; set; }
		public static int HeartbeatInterval { get; set; }
		public static string HubToken { get; set; }
		public static string NodeType { get; set; }

		public static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseProxy = true,
			UseCookies = false
		});

		static Env()
		{
			IsRunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

			RunningLockPath = Path.Combine(AppContext.BaseDirectory, "agent.lock");
			NodeIdPath = Path.Combine(AppContext.BaseDirectory, "nodeId");
			ProjectsDirectory = Path.Combine(AppContext.BaseDirectory, "projects");
			PackagesDirectory = Path.Combine(AppContext.BaseDirectory, "packages");
			ProcessesDirectory = Path.Combine(AppContext.BaseDirectory, "proc");

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
			if (!Directory.Exists(ProcessesDirectory))
			{
				Directory.CreateDirectory(ProcessesDirectory);
			}
			HostName = Dns.GetHostName();
			var interf = NetworkInterface.GetAllNetworkInterfaces().First(i => i.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
			var unicastAddresses = interf.GetIPProperties().UnicastAddresses;
			Ip = unicastAddresses.First(a => a.IPv4Mask.ToString() != "255.255.255.255" && a.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();
			NodeId = Ip;
		}

		public static void Load()
		{
			var builder = new ConfigurationBuilder();
			builder.AddIniFile("config.ini");

			_configuration = builder.Build();

			ServerUrl = _configuration.GetValue<string>("serverUrl");
			HeartbeatInterval = _configuration.GetValue<int>("heartbeatInterval");
			HeartbeatUrl = $"{ServerUrl}api/v1.0/nodeheartbeat";
			HubToken = _configuration.GetValue<string>("hubToken");
			NodeType = _configuration.GetValue<string>("type");
		}
	}
}
