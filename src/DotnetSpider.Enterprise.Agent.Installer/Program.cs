using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace DotnetSpider.Enterprise.Agent.Installer
{
	class Program
	{
		static HttpClient client = new HttpClient();
		static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				Console.WriteLine("Agent type is missing.");
				return;
			}

			KillProcess();
			RmoveAgentLock();
			DownloadExtractPackage();
			UpdateConfiguration(args.First());
			StartProgrom();
		}

		private static void UpdateConfiguration(string type)
		{
			switch (type.ToLower())
			{
				case "vps":
					{
						var config = client.GetStringAsync("http://nasabigdata.com:30012/contents/dotnetspider.enterprise/config.vps.ini").Result;
						File.WriteAllText("/opt/DotnetSpider.Agent/config.ini", config);
						break;
					}
				case "default":
					{
						var config = client.GetStringAsync("http://nasabigdata.com:30012/contents/dotnetspider.enterprise/config.ini").Result;
						File.WriteAllText("/opt/DotnetSpider.Agent/config.ini", config);
						break;
					}
			}

		}

		static void StartProgrom()
		{
			Console.WriteLine("尝试启动程序...");

			var proces = Process.Start(new ProcessStartInfo
			{
				FileName = "sh",
				Arguments = "/opt/DotnetSpider.Agent/start.daemon.sh",
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				WorkingDirectory = "/opt/DotnetSpider.Agent",

			});
			proces.WaitForExit();

			Console.WriteLine("启动程序完成...");
		}

		static void DownloadExtractPackage()
		{
			Console.WriteLine("正在请求 http://nasabigdata.com:30012/contents/dotnetspider.enterprise/ 查找程序包...");
			var content = client.GetAsync("http://nasabigdata.com:30012/contents/dotnetspider.enterprise/").Result.Content.ReadAsStringAsync().Result;
			var matches = Regex.Matches(content, "/contents/dotnetspider.enterprise/(?<package>DotnetSpider.Agent\\d+.zip)");
			var packageName = string.Empty;
			foreach (Match match in matches)
			{
				var package = match.Groups["package"].Value;
				if (package.CompareTo(packageName) > 0)
				{
					packageName = package;
				}
			}
			Console.WriteLine($"获取最新程序包 {packageName}");

			Console.WriteLine($"正在删除 /tmp/{packageName}...");
			File.Delete($"/tmp/{ packageName}");
			Console.WriteLine($"已成功清理/tmp/{packageName}。");

			Console.WriteLine("正在下载写入程序包...");
			var bytes = client.GetByteArrayAsync($"http://nasabigdata.com:30012/contents/dotnetspider.enterprise/{packageName}").Result;
			using (var fileStream = new FileStream($"/tmp/{packageName}", FileMode.CreateNew))
			{
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Flush();
				fileStream.Close();
			}
			Console.WriteLine("正在解压缩...");
			System.IO.Compression.ZipFile.ExtractToDirectory($"/tmp/{packageName}", "/opt/");
			Console.WriteLine("解压缩完成...");
		}

		static void RmoveAgentLock()
		{
			Console.WriteLine("正在删除DotnetSpider.Enterprise.Agent...");
			Directory.Delete("/opt/DotnetSpider.Agent", true);
			Console.WriteLine("已清理DotnetSpider.Enterprise.Agent文件夹。");
		}

		static void KillProcess()
		{
			Console.WriteLine("正在查找DotnetSpider.Enterprise.Agent进程...");

			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				Console.WriteLine("Not supported!");
			}
			else
			{
				var dirs = Directory.GetDirectories("/proc/");
				foreach (var dir in dirs)
				{
					int number;
					var dirName = dir.Replace("/proc/", "").Replace("/", "");
					if (int.TryParse(dirName, out number))
					{
						var argumentsText = File.ReadAllText($"/proc/{dirName}/cmdline");
						if (argumentsText.Contains("DotnetSpider.Enterprise.Agent.dll"))
						{
							Process.Start("kill", $"-s 9 {dirName}").WaitForExit();
							Console.WriteLine($"killed process:{dirName}");
						}
					}
				}
			}

			Console.WriteLine("已清理DotnetSpider.Enterprise.Agent进程。");
		}
	}
}
