using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetSpider.Enterprise.Agent
{
	public class Config
	{
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
		public static int NodeType { get; set; }

		public static readonly HttpClient HttpClient = new HttpClient();

		static Config()
		{
			IsRunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

			RunningLockPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "agent.lock");
			NodeIdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nodeId");
			ProjectsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects");
			PackagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "packages");
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
			var interf = NetworkInterface.GetAllNetworkInterfaces().First(i => i.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
			var unicastAddresses = interf.GetIPProperties().UnicastAddresses;
			Ip = unicastAddresses.First(a => a.IPv4Mask.ToString() != "255.255.255.255" && a.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();
			NodeId = Ip;
			CpuFullLoad = 0.8 * Environment.ProcessorCount;
		}

		internal static void Load()
		{
			var lines = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "config.ini"));
			var dic = new Dictionary<string, string>();
			foreach (var v in lines)
			{
				var info = v.Split('=');
				dic[info[0]] = info[1];
			}
			PackageUrl = dic["packageUrl"];
			ServerUrl = dic["serverUrl"];
			HeartbeatInterval = int.Parse(dic["heartbeatInterval"]);
			HeartbeatUrl = $"{ServerUrl}node/heartbeat";
			ApiToken = dic["apiToken"];
			NodeType = int.Parse(dic["type"]);
		}
	}
}
