using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
	public class AgentClient
	{
		private static ILogger Logger;
		private Task _task;
		private int step = 0;
		private bool _exit;
		private FileStream _lockFileStream;
		private readonly Ping Ping = new Ping();

		public AgentClient()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Console.Title = $"DotnetSpider Agent v{Config.Version}";
			}
		}

		public void Run(params string[] args)
		{
			CheckIfOtherProcessExists();
			_lockFileStream = File.Create(Config.RunningLockPath);

			CheckConfig();
			LoadConfig();
			MonitorErrorDialogOnWindows();
			CommandExecutor.OnExited += CommandExecutor_OnExited;
			if (args.Contains("--daemon"))
			{
				Start();
			}
			else
			{
				StartAysnc();
				Console.WriteLine("Enter q: to exit:");
				while (Console.ReadLine() != "q:")
				{
					Console.WriteLine("Press q: to exit.");
				}
				CommandExecutor.Exit();
			}
		}

		private void CheckIfOtherProcessExists()
		{
			if (File.Exists(Config.RunningLockPath))
			{
				try
				{
					File.Delete(Config.RunningLockPath);
				}
				catch (Exception)
				{
					// 此处方法还未检查NLog配置文件, 因此不能使用日志。
					Console.WriteLine("Agent is running.");
					Environment.Exit(1);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void CheckConfig()
		{
			string nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
			if (!File.Exists(nlogConfigPath))
			{
				Console.WriteLine("NLog configuraiton file nlog.config unfound.");
				Environment.Exit(1);
			}
			Logger = LogManager.GetCurrentClassLogger();
			string configPath = Path.Combine(AppContext.BaseDirectory, "config.ini");
			if (!File.Exists(configPath))
			{
				Logger.Error("Agent configuration file config.ini unfound.");
				Environment.Exit(1);
			}

			Logger.Info($"[{++step}] Check configuration exists success.");
		}

		/// <summary>
		/// 
		/// </summary>
		private void LoadConfig()
		{
			string nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
			LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigPath);

			Config.Load();

			Logger.Info($"[{++step}] Load configuration success.");
		}

		/// <summary>
		/// 
		/// </summary>
		private void Start()
		{
			while (!_exit)
			{
				if (IsInternetOk())
				{
					Heartbeat();
				}
				Thread.Sleep(Config.HeartbeatInterval);
			}
		}

		private void StartAysnc()
		{
			_task = Task.Factory.StartNew(() =>
			{
				Start();
			});
		}

		private async void Heartbeat()
		{
			try
			{
				var hearbeat = HeartBeat.Create();
				var json = JsonConvert.SerializeObject(hearbeat);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				await Config.HttpClient.PostAsync(Config.HeartbeatUrl, content).ContinueWith((task) =>
				{
					HttpResponseMessage response = task.Result;
					response.EnsureSuccessStatusCode();
					var result = response.Content.ReadAsStringAsync().Result;
					if (!string.IsNullOrEmpty(result))
					{
						var commands = JsonConvert.DeserializeObject<Messsage[]>(result);
						foreach (var command in commands)
						{
							CommandExecutor.Execute(command);
						}
					}
				});

				Logger.Trace(hearbeat);
			}
			catch (Exception e)
			{
				Logger.Error($"Heartbeart failed: {e}");
			}
		}

		private void CommandExecutor_OnExited()
		{
			_exit = true;
			_task?.Wait();

			_lockFileStream?.Dispose();

			try
			{
				File.Delete(Config.RunningLockPath);
			}
			catch (Exception e)
			{
				Logger.Info($"Delete process lock failed: {e}");
			}
		}

		private void MonitorErrorDialogOnWindows()
		{
			if (Config.IsRunningOnWindows)
			{
				Logger.Info($"[{++step}] Start monitor error dialog.");

				Task.Factory.StartNew(() =>
				{
					while (!_exit)
					{
						try
						{
							var errorDialogs = Process.GetProcessesByName("WerFault");
							foreach (var errorDialog in errorDialogs)
							{
								errorDialog.Kill();
							}
						}
						catch (Exception e)
						{
							Logger.Info($"Kill error dialog failed: {e}");
						}
						Thread.Sleep(1500);
					}
				});
			}
		}

		private bool IsInternetOk()
		{
			try
			{
				PingReply pr = Ping.Send("www.baidu.com", 5000);

				return (pr != null && pr.Status == IPStatus.Success);
			}
			catch
			{
				return false;
			}
		}
	}
}
