using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DotnetSpider.Enterprise.Agent
{
    public class NodeStatus
    {
        /// <summary>
        /// IP Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Cpu Load
        /// </summary>
        public int CpuLoad { get; set; }
        /// <summary>
        /// Free Memeroy
        /// </summary>
        public long FredMemeroy { get; set; }
        /// <summary>
        /// TotalMemeory
        /// </summary>
        public long TotalMemory { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public long Timestamp { get; set; }

        public int CountOfRunningTasks { get; set; }

        public bool IsEnabled { get; set; }

        public string Os { get; set; }

        public static readonly string Version = "0.9.9";

        public override string ToString()
        {
            return $"{Address}|{CpuLoad}|{FredMemeroy}|{TotalMemory}|{Timestamp}|{IsEnabled}|{CountOfRunningTasks}|{Version}|{Os}";
        }

        public static NodeStatus Current()
        {
            MemoryInfo mInfo = GetMemoryInfo();
            if (mInfo != null)
            {
                var hostName = Dns.GetHostName();
                var status = new NodeStatus
                {
                    Address = GetIpAddress(hostName),
                    CpuLoad = Convert.ToInt32(GetCpuLoad()),
                    FredMemeroy = mInfo.FreeMemory,
                    TotalMemory = mInfo.TotalMemory,
                    Timestamp = DateTime.UtcNow.Ticks,//此处应该拿服务器的时间
                    IsEnabled = AgentConsts.IsEnabled,
                    CountOfRunningTasks = HandlerFactory.GetRunningCount(),
                    Os = RuntimeInformation.OSDescription
                };
                return status;
            }
            return null;
        }

        internal class MemoryInfo
        {
            public long TotalMemory { get; set; }
            public long FreeMemory { get; set; }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
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

        public static string GetIpAddress()
        {
            return GetIpAddress(Dns.GetHostName());
        }

        private static string GetIpAddress(string hostName)
        {
            if (AgentConsts.IsRunningOnWindows)
            {
                var addressList = Dns.GetHostAddressesAsync(hostName).Result;
                IPAddress localaddr = addressList.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToList()[0];
                var ip = localaddr.ToString();
                return ip;
            }
            else
            {
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = "ip",
                        Arguments = "address",
                        CreateNoWindow = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true
                    }
                };
                process.Start();
                string info = process.StandardOutput.ReadToEnd();
                var lines = info.Split('\n');
                string ip = string.Empty;
                foreach (var line in lines)
                {
                    var content = line.Trim();
                    if (!string.IsNullOrEmpty(content) && Regex.IsMatch(content, @"^inet.*$"))
                    {
                        var str = Regex.Match(content, @"[0-9]{0,3}\.[0-9]{0,3}\.[0-9]{0,3}\.[0-9]{0,3}/[0-9]+").Value;
                        if (!string.IsNullOrEmpty(str) && !str.Contains("127.0.0.1"))
                        {
                            ip = str.Split('/')[0];
                        }
                    }
                }
                process.WaitForExit();
                process.Dispose();
                return ip;
            }
        }

        private static MemoryInfo GetMemoryInfo()
        {
            MemoryInfo mInfo = null;
            if (AgentConsts.IsRunningOnWindows)
            {
                MEMORYSTATUS mStatus = new MEMORYSTATUS();
                GlobalMemoryStatus(ref mStatus);
                mInfo = new MemoryInfo
                {
                    FreeMemory = Convert.ToInt64(mStatus.dwAvailPhys) / 1024 / 1024,
                    TotalMemory = Convert.ToInt64(mStatus.dwTotalPhys) / 1024 / 1024
                };
                return mInfo;
            }
            else
            {
                Process process = new Process
                {
                    StartInfo =
                    {
                        FileName = "free",
                        CreateNoWindow = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true
                    }
                };
                process.Start();
                string info = process.StandardOutput.ReadToEnd();
                var lines = info.Split('\n');

                foreach (var line in lines)
                {
                    var content = line.Trim();
                    if (!string.IsNullOrEmpty(content) && Regex.IsMatch(content, @"^Mem.*$"))
                    {
                        var cols = content.Split(' ').Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
                        if (cols.Count >= 4)
                        {
                            int freeMem;
                            int.TryParse(cols[3], out freeMem);

                            int totalMem;
                            int.TryParse(cols[1], out totalMem);

                            mInfo = new MemoryInfo
                            {
                                FreeMemory = freeMem,
                                TotalMemory = totalMem
                            };
                            break;
                        }
                    }
                }
                process.WaitForExit();
                process.Dispose();
            }

            return mInfo;
        }

        private static decimal GetCpuLoad()
        {
            if (AgentConsts.IsRunningOnWindows)
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
    }
}