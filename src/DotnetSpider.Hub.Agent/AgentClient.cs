﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotnetSpider.Hub.Agent.Command;
using Newtonsoft.Json;
using Serilog;

namespace DotnetSpider.Hub.Agent
{
	public class AgentClient : IDisposable
	{
		private Task _task;
		private int _step;
		private FileStream _singletonLock;
		private readonly Ping _ping = new Ping();
		private readonly string _config;
		private readonly bool _daemon = false;
		private static string _configPath = Path.Combine(AppContext.BaseDirectory, "config.ini");

		public bool HasExited { get; private set; }

		public AgentClient(Options config)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Console.Title = $"DotnetSpider Hub Agent v{Env.Version}";
			}

			if (Uri.TryCreate(config.Config, UriKind.RelativeOrAbsolute, out _))
			{
				_config = config.Config;
			}

			_daemon = config.Daemon;
		}

		public void Run()
		{
			CheckIfOtherProcessIsRunning();
			CheckConfig();
			LoadConfig();
			MonitorErrorDialogOnWindows();
			if (_daemon) //后台
            {
				Start(); 
			}
			else//前台交互运行
			{
                //真正的运行
				StartAysnc();
				Console.WriteLine("Enter q: to exit:");
				while (Console.ReadLine() != "q:")
				{
					Console.WriteLine("Press q: to exit.");
				}
				CommandExecutor.Execute(new Messsage { Name = CommandNames.ExitName }, this);
			}
		}

		public void Dispose()
		{
			HasExited = true;
			_task?.Wait();

			_singletonLock?.Dispose();

			try
			{
				File.Delete(Env.RunningLockPath);
			}
			catch (Exception e)
			{
				Log.Logger.Information($"Delete process lock failed: {e}");
			}
		}

		private void CheckIfOtherProcessIsRunning()
		{
			if (File.Exists(Env.RunningLockPath))
			{
				try
				{
					File.Delete(Env.RunningLockPath);
				}
				catch (Exception)
				{
					// 此处方法还未检查NLog配置文件, 因此不能使用日志。
					Console.WriteLine("Agent is running.");
					Environment.Exit(1);
				}
			}
			_singletonLock = File.Create(Env.RunningLockPath);
		}

		/// <summary>
		/// 
		/// </summary>
		private void CheckConfig()
		{
			if (!string.IsNullOrWhiteSpace(_config))
			{
				var bytes = Env.HttpClient.GetByteArrayAsync(_config).Result;
				File.WriteAllBytes(_configPath, bytes);
			}
			if (!File.Exists(_configPath))
			{
				Log.Logger.Error("Agent configuration file config.ini unfound.");
				Environment.Exit(1);
			}

			Log.Logger.Information($"[{++_step}] Check configuration exists success.");
		}

		/// <summary>
		/// 
		/// </summary>
		private void LoadConfig()
		{
			Env.Load();

			Log.Logger.Information($"[{++_step}] Load configuration success.");
		}

		/// <summary>
		/// 运行
		/// </summary>
		private void Start()
		{
			while (!HasExited)
			{
				if (IsInternetOk())
				{
					Heartbeat();
				}
				Thread.Sleep(Env.HeartbeatInterval);
			}
		}

        /// <summary>
        /// 异步运行
        /// </summary>
		private void StartAysnc()
		{
            //运行
			_task = Task.Factory.StartNew(Start);
		}

        /// <summary>
        /// 心跳
        /// </summary>
		private void Heartbeat()
		{
			try
			{
				var hearbeat = HeartBeat.Create();
				var json = JsonConvert.SerializeObject(hearbeat);
				HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Env.HeartbeatUrl);
				httpRequestMessage.Headers.Add("HubToken", Env.HubToken);
				httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

				Env.HttpClient.SendAsync(httpRequestMessage).ContinueWith((task) =>
				{
					HttpResponseMessage response = task.Result;
					response.EnsureSuccessStatusCode();
					var result = response.Content.ReadAsStringAsync().Result;
					if (!string.IsNullOrEmpty(result))
					{
						var jobj = JsonConvert.DeserializeObject<StandardResult>(result);
						if (jobj.Status == Status.Success && jobj.Data != null)
						{
							foreach (var command in jobj.Data.ToObject<Messsage[]>())
							{
								ThreadPool.QueueUserWorkItem((obj) =>
								{
									CommandExecutor.Execute(command, this);
								});
							}
						}
						else
						{
							Log.Logger.Error($"Heartbeart failed: {jobj.Message}");
						}
					}
				}).Wait();

				Log.Logger.Verbose(hearbeat.ToString());
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Heartbeart failed: {e}");
			}
		}

        /// <summary>
        /// 监控windows上的错误对话
        /// </summary>
		private void MonitorErrorDialogOnWindows()
		{
			if (Env.IsRunningOnWindows)
			{
				Log.Logger.Information($"[{++_step}] Start monitor error dialog.");

				Task.Factory.StartNew(() =>
				{
					while (!HasExited)
					{
						try
						{
							var errorDialogs = System.Diagnostics.Process.GetProcessesByName("WerFault");
							foreach (var errorDialog in errorDialogs)
							{
								errorDialog.Kill();
							}
						}
						catch (Exception e)
						{
							Log.Logger.Information($"Kill error dialog failed: {e}");
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
				PingReply pr = _ping.Send("www.baidu.com", 5000);
				return pr != null && pr.Status == IPStatus.Success;
			}
			catch
			{
				return false;
			}
		}
	}
}
