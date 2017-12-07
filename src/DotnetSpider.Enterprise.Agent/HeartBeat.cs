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
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				CPUUsageSearch = new ManagementObjectSearcher("SELECT *  FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'");
			}
		}

		public static HeartBeat Create()
		{
			var heartBeat = new HeartBeat
			{
				NodeId = Config.NodeId,
				Ip = Config.Ip,
				CPULoad = Convert.ToInt32(GetCpuLoad()),
				Os = Config.Os,
				Version = Config.Version,
				CPUCoreCount = Environment.ProcessorCount,
				Type = Config.NodeType,
				ProcessCount = CommandExecutor.ProcessCount
			};

			if (Config.IsRunningOnWindows)
			{
#if NET45
				Microsoft.VisualBasic.Devices.ComputerInfo info = new Microsoft.VisualBasic.Devices.ComputerInfo();
				heartBeat.FreeMemory = Convert.ToInt64(info.AvailablePhysicalMemory) / 1024 / 1024;
				heartBeat.TotalMemory = Convert.ToInt64(info.TotalPhysicalMemory) / 1024 / 1024;
#endif
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
			if (Config.IsRunningOnWindows)
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
				var line = File.ReadAllText("/proc/loadavg");
				var loadAvgs = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Take(1).ToList();
				double load5sec = double.Parse(loadAvgs[0]);
				return (decimal)((load5sec / Config.CpuFullLoad) * 100);
			}
		}
	}
}
