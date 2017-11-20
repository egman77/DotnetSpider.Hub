using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;


namespace DotnetSpider.Enterprise.Agent.Installer
{
	class Program
	{
		static HttpClient client = new HttpClient();
		static void Main(string[] args)
		{
			KillProcess();
			RmoveAgentLock();
			DownloadExtractPackage();
			StartProgrom();

			Console.Write("Process any key to continue..");
			Console.Read();
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

			var proces = Process.Start(new ProcessStartInfo
			{
				FileName = "rm",
				Arguments = "/opt/DotnetSpider.Agent -rf",
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			});
			proces.WaitForExit();

			Console.WriteLine("已清理DotnetSpider.Enterprise.Agent文件夹。");
		}

		static void KillProcess()
		{
			Console.WriteLine("正在查找DotnetSpider.Enterprise.Agent进程...");

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				Console.WriteLine("Not supported!");
			}
			else
			{
				var proces = Process.Start(new ProcessStartInfo
				{
					FileName = "ps",
					Arguments = "-ef",
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				});
				proces.WaitForExit();
				while (!proces.StandardOutput.EndOfStream)
				{
					var info = proces.StandardOutput.ReadLine();
					if (string.IsNullOrEmpty(info)) break;
					var splitArr = info.Split(' ', StringSplitOptions.RemoveEmptyEntries);
					if (splitArr.Length > 8)
					{
						if (splitArr[8] == "DotnetSpider.Enterprise.Agent.dll")
						{
							Process.Start("kill", $"-s 9 {splitArr[1]}");
							Console.WriteLine($"killed process:{splitArr[1]}");
						}
					}
				}
				proces.Close();
			}
			

			Console.WriteLine("已清理DotnetSpider.Enterprise.Agent进程。");
		}
	}
}
