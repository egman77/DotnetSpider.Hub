using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace DotnetSpider.Hub.Agent
{
	public static class Util
	{
		private struct Memorystatus
		{
			public uint DwLength { get; set; }
			public uint DwMemoryLoad { get; set; }
			public UInt64 DwTotalPhys { get; set; } //总的物理内存大小
			public UInt64 DwAvailPhys { get; set; } //可用的物理内存大小 
			public UInt64 DwTotalPageFile { get; set; }
			public UInt64 DwAvailPageFile { get; set; } //可用的页面文件大小
			public UInt64 DwTotalVirtual { get; set; } //返回调用进程的用户模式部分的全部可用虚拟地址空间
			public UInt64 DwAvailVirtual { get; set; } // 返回调用进程的用户模式部分的实际自由可用的虚拟地址空间
		}

		private static readonly ManagementObjectSearcher CpuUsageSearch;
		private static readonly bool IsServer2008;

		public static readonly long TotalMemory;

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GlobalMemoryStatus(ref Memorystatus lpBuffer);

		static Util()
		{
			IsServer2008 = RuntimeInformation.OSDescription.Contains("6.1.7");
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !IsServer2008)
			{
				CpuUsageSearch = new ManagementObjectSearcher("SELECT *  FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'");
			}

			if (Env.IsRunningOnWindows)
			{
				Memorystatus mStatus = new Memorystatus();
				GlobalMemoryStatus(ref mStatus);
				TotalMemory = Convert.ToInt64(mStatus.DwTotalPhys) / 1024 / 1024;
			}
			else
			{
				var lines = File.ReadAllLines("/proc/meminfo");
				var infoDic = new Dictionary<string, long>();
				foreach (var line in lines)
				{
					var datas = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Take(2).ToList();
					infoDic.Add(datas[0], long.Parse(datas[1]));
				}
				TotalMemory = infoDic["MemTotal:"] / 1024;
			}

		}

		public static decimal GetCpuLoad()
		{
			decimal total = 100;
			if (Env.IsRunningOnWindows)
			{
				if (!IsServer2008)
				{
					var s = CpuUsageSearch.Get();
					foreach (var o in s)
					{
						var obj = (ManagementObject)o;
						var usage = obj["PercentIdleTime"];
						total -= decimal.Parse(usage.ToString());
					}
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
						total = decimal.Parse(loadStr);
					}
					else
					{
						total =  99;
					}
				}
			}
			else
			{
				total= CpuUsage.Current;
			}

			return total;
		}

		public static long GetFreeMemory()
		{
			if (Env.IsRunningOnWindows)
			{
				Memorystatus mStatus = new Memorystatus();
				GlobalMemoryStatus(ref mStatus);
				return Convert.ToInt64(mStatus.DwAvailPhys) / 1024 / 1024;
			}
			else
			{
				var lines = File.ReadAllLines("/proc/meminfo");
				var infoDic = new Dictionary<string, long>();
				foreach (var line in lines)
				{
					var datas = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Take(2).ToList();
					infoDic.Add(datas[0], long.Parse(datas[1]));
				}
				var free = infoDic["MemFree:"];
				var sReclaimable = infoDic["SReclaimable:"];
				return (free + sReclaimable) / 1024;
			}
		}
	}
}
