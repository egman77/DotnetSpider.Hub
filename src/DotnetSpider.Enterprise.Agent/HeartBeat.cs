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
		public virtual int CPUCoreCount { get; set; }

		public static HeartBeat Create()
		{
			var heartBeat = new HeartBeat
			{
				NodeId = Config.NodeId,
				Ip = Config.Ip,
				CPULoad = Convert.ToInt32(GetCpuLoad()),
				Os = Config.Os,
				Version = Config.Version,
				CPUCoreCount = Environment.ProcessorCount
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
				var infoDic = new Dictionary<string, long>();
				foreach (var line in lines)
				{
					var datas = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Take(2).ToList();
					infoDic.Add(datas[0], long.Parse(datas[1]));
				}
				var free = infoDic["MemFree:"];
				var sReclaimable = infoDic["SReclaimable:"];
				heartBeat.FreeMemory = free + sReclaimable / 1024;
				heartBeat.TotalMemory = infoDic["MemTotal:"] / 1024;
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
				var line = File.ReadAllText("/proc/loadavg");
				var loadAvgs = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Take(1).ToList();
				double load5sec = double.Parse(loadAvgs[0]);
				return (decimal)((load5sec / Config.CpuFullLoad) * 100);
			}
			return 100;
		}

		public override string ToString()
		{
			return $"{NodeId}, {CPULoad}, {FreeMemory}, {TotalMemory}, {ProcessCount}, {CPUCoreCount}";
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
