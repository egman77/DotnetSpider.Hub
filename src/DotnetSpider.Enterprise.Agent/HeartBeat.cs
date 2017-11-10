using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetSpider.Enterprise.Agent
{
	public class HeartBeat
	{
		public virtual string NodeId { get; set; }
		public virtual string Ip { get; set; }
		public virtual int CPULoad { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual int ProcessCount { get; set; }
		public virtual string Os { get; set; }
		public virtual string Version { get; set; }

		public static HeartBeat Create()
		{
			var heartBeat = new HeartBeat
			{
				NodeId = Config.NodeId,
				Ip = Config.Ip,
				CPULoad = Convert.ToInt32(GetCpuLoad()),
				Os = Config.Os,
				Version = Config.Version
			};

			if (Config.IsRunningOnWindows)
			{
				MEMORYSTATUS mStatus = new MEMORYSTATUS();
				GlobalMemoryStatus(ref mStatus);
				heartBeat.FreeMemory = Convert.ToInt64(mStatus.dwAvailPhys) / 1024 / 1024;
				heartBeat.TotalMemory = Convert.ToInt64(mStatus.dwTotalPhys) / 1024 / 1024;
			}
			else
			{
				var lines = File.ReadAllLines("/proc/meminfo");
				heartBeat.FreeMemory = int.Parse(lines[1]) / 1024;
				heartBeat.TotalMemory = int.Parse(lines[0]) / 1024;
			}
			return heartBeat;
		}

		private static decimal GetCpuLoad()
		{
			if (Config.IsRunningOnWindows)
			{
				Process process = new Process
				{
					StartInfo =
					{
						FileName = "wmic",
						Arguments = "cpu get LoadPercentage",
						CreateNoWindow = false,
						RedirectStandardOutput = true,
						RedirectStandardInput = true
					}
				};
				process.Start();
				string info = process.StandardOutput.ReadToEnd();
				var lines = info.Split('\n');

				if (lines.Length > 1)
				{
					var loadStr = lines[1].Trim();
					process.WaitForExit();
					process.Dispose();
					return decimal.Parse(loadStr);
				}
				else
				{
					process.WaitForExit();
					process.Dispose();
				}
			}
			else
			{
				Process process = new Process
				{
					StartInfo =
					{
						FileName = "top",
						Arguments = "-bn 1",
						CreateNoWindow = false,
						RedirectStandardOutput = true,
						RedirectStandardInput = true
					}
				};
				process.Start();
				string info = process.StandardOutput.ReadToEnd();
				var lines = info.Split('\n');

				decimal cpuLoad = 0;
				foreach (var line in lines)
				{
					var content = line.Trim();
					if (!string.IsNullOrEmpty(content) && Regex.IsMatch(content, @"^\d+.*$"))
					{
						var cols = content.Split(' ').Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
						if (cols.Count >= 8)
						{
							cpuLoad += decimal.Parse(cols[8].Trim());
						}
					}
				}
				process.WaitForExit();
				process.Dispose();

				return cpuLoad;
			}
			return 100;
		}

		public struct MEMORYSTATUS
		{
			public uint dwLength;
			public uint dwMemoryLoad;
			public UInt64 dwTotalPhys; //总的物理内存大小
			public UInt64 dwAvailPhys; //可用的物理内存大小 
			public UInt64 dwTotalPageFile;
			public UInt64 dwAvailPageFile; //可用的页面文件大小
			public UInt64 dwTotalVirtual; //返回调用进程的用户模式部分的全部可用虚拟地址空间
			public UInt64 dwAvailVirtual; // 返回调用进程的用户模式部分的实际自由可用的虚拟地址空间

		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);
	}
}
