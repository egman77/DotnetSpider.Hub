﻿using System;
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
        /// <summary>
        /// 基础数据目录
        /// </summary>
		public static string BaseDataDirectory { get; set; }
        /// <summary>
        /// 运行锁路径
        /// </summary>
		public static string RunningLockPath { get; set; }
        /// <summary>
        /// 节点ID路径
        /// </summary>
		public static string NodeIdPath { get; set; }
        /// <summary>
        /// 项目目录
        /// </summary>
		public static string ProjectsDirectory { get; set; }
        /// <summary>
        /// 包目录
        /// </summary>
		public static string PackagesDirectory { get; set; }
        /// <summary>
        /// 处理器目录
        /// </summary>
		public static string ProcessesDirectory { get; set; }
        /// <summary>
        /// 是否运行在windows下
        /// </summary>
		public static bool IsRunningOnWindows { get; }
        /// <summary>
        /// 节点id
        /// </summary>
		public static string NodeId { get; set; }
        /// <summary>
        /// 节点ip
        /// </summary>
		public static string Ip { get; set; }
        /// <summary>
        /// 节点主机名
        /// </summary>
		public static string HostName { get; set; }
        /// <summary>
        /// 节点操作系统
        /// </summary>
		public static string Os { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
		public const string Version = "1.0.0";
        /// <summary>
        /// 服务器url
        /// </summary>
		public static string ServerUrl { get; set; }
        /// <summary>
        /// 心跳url
        /// </summary>
		public static string HeartbeatUrl { get; set; }
        /// <summary>
        /// 心跳间隔
        /// </summary>
		public static int HeartbeatInterval { get; set; }
        /// <summary>
        /// 集线器(调度系统)令牌
        /// </summary>
		public static string HubToken { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
		public static string NodeType { get; set; }

        public static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true,
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            UseProxy = true,
            UseCookies = false
        });


        /// <summary>
        /// 构造环境信息
        /// </summary>
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
            HostName = Dns.GetHostName(); //获得主机名


        }



        /// <summary>
        /// 从配置中加载环境信息
        /// </summary>
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

            NodeId = GetHostIpv4(_configuration.GetValue<string>("HostInterfaceName")); //获得ip地址
        }

        /// <summary>
        /// 获取宿主的ip地址
        /// </summary>
        /// <returns></returns>
        static string GetHostIpv4(string hostInterfaceName)
        {
            //string[] names = { "本地", "local", };

            //如果没有设置,猜测
            if (string.IsNullOrWhiteSpace(hostInterfaceName))
            {
               return  GuessHostIpv4();
            }

            var interfs = NetworkInterface.GetAllNetworkInterfaces();


            //var interf = NetworkInterface.GetAllNetworkInterfaces().First(i => i.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
            //精确匹配
            var interf= interfs.First(i => i.Name.Contains(hostInterfaceName));
            if(interf==null)
            {
                //模糊匹配
                interf = interfs.First(i => i.Name.IndexOf(hostInterfaceName)!=-1);
            }
            else if(interf==null)
            {
                //猜测
                return GuessHostIpv4();
            }


            var unicastAddresses = interf.GetIPProperties().UnicastAddresses;
            var ip = unicastAddresses.First(a => a.IPv4Mask.ToString() != "255.255.255.255" && a.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();

            return ip;
        }

        /// <summary>
        /// 猜测宿主的ip地址
        /// </summary>
        /// <returns></returns>
        static string GuessHostIpv4()
        {
            NetworkInterface interf = null;

            var interfs = NetworkInterface.GetAllNetworkInterfaces();

            if(interfs.Count()==1)
            {
                interf = interfs[0];
                return GetHostIpv4(interf);
            }
           
           //猜接上网络的   
           var interfs2 = interfs.Where(i => i.OperationalStatus == OperationalStatus.Up );

            if(interfs2.Count()==1)
            {
                interf = interfs2.First();
                return GetHostIpv4(interf);
            }
           
            //猜发包数最大的
            var max = interfs2.Max(j => j.GetIPv4Statistics().UnicastPacketsSent);
            var interfs3= interfs2.Where(i=>i.GetIPv4Statistics().UnicastPacketsSent >= max);

            if(interfs3.Count()==1)
            {
                interf = interfs3.First();
                return GetHostIpv4(interf);
            }

            //猜接口类型
            interf = interfs3.First(i => i.NetworkInterfaceType == NetworkInterfaceType.Ethernet||
                i.NetworkInterfaceType==NetworkInterfaceType.Wireless80211);

            //取环回接口
            if(interf==null)
            {
                interf = interfs.First(i => i.NetworkInterfaceType == NetworkInterfaceType.Loopback );
            }
            return GetHostIpv4(interf);
        }

           
          
        

    static string GetHostIpv4(NetworkInterface interf)
    {
        var unicastAddresses = interf.GetIPProperties().UnicastAddresses;
        var ip = unicastAddresses.First(a => a.IPv4Mask.ToString() != "255.255.255.255" && a.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();
        return ip;
    }
    }
}
