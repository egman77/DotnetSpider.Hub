using DotnetSpider.Enterprise.Agent.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetSpider.Enterprise.Agent
{
	public class HeartBeat
	{
		private static readonly ManagementObjectSearcher CPUUsageSearch;
		private static readonly bool IsServer2008;

		public virtual string NodeId { get; set; }
		public virtual string Ip { get; set; }
		public virtual int CPULoad { get; set; }
		public virtual long FreeMemory { get; set; }
		public virtual long TotalMemory { get; set; }
		public virtual int ProcessCount { get; set; }
		public virtual int Type { get; set; }
		public virtual string Os { get; set; }
		public virtual string Version { get; set; }
		public virtual int CPUCoreCount { get; set; }

		static HeartBeat()
		{
			IsServer2008 = RuntimeInformation.OSDescription.Contains("6.1.7");
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsServer2008)
			{
				CPUUsageSearch = new ManagementObjectSearcher("SELECT *  FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'");
			}
		}

		public static HeartBeat Create()
		{
			var heartBeat = new HeartBeat
			{
				NodeId = Env.NodeId,
				Ip = Env.Ip,
				CPULoad = Convert.ToInt32(GetCpuLoad()),
				Os = Env.Os,
				Version = Env.Version,
				CPUCoreCount = Environment.ProcessorCount,
				Type = Env.NodeType,
				ProcessCount = ProcessManager.ProcessCount
			};

			if (Env.IsRunningOnWindows)
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
				heartBeat.FreeMemory = (free + sReclaimable) / 1024;
				heartBeat.TotalMemory = infoDic["MemTotal:"] / 1024;
			}
			return heartBeat;
		}

		public override string ToString()
		{
			return $"{NodeId}, {CPULoad}, {FreeMemory}, {TotalMemory}, {ProcessCount}, {CPUCoreCount}";
		}

		private static decimal GetCpuLoad()
		{
			if (Env.IsRunningOnWindows)
			{
				if (!IsServer2008)
				{
					decimal total = 100;
					var s = CPUUsageSearch.Get();
					foreach (ManagementObject obj in s)
					{
						var usage = obj["PercentIdleTime"];
						total -= decimal.Parse(usage.ToString());
					}
					return total;
				}
				else
				{
					var process = new System.Diagnostics.Process
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
					process.WaitForExit();
					process.Dispose();
					if (lines.Length > 1)
					{
						var loadStr = lines[1].Trim();
						return decimal.Parse(loadStr);
					}
					else
					{
						return 99;
					}
				}
			}
			else
			{
				return CpuUsage.Current;
			}
		}

		private struct MEMORYSTATUS
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
		private static extern bool GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);
	}
}
