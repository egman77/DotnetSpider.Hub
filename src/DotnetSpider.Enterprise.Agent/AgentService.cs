using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
	public class AgentService
	{
		private static readonly ConcurrentDictionary<long, ProcessInfo> Processes = new ConcurrentDictionary<long, ProcessInfo>();
		private static ILogger _logger;
		private static readonly HttpClient httpClient = new HttpClient();
		private Task _task;
		private int step = 0;
		private bool _exit;
		private static ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					_logger = LogManager.GetCurrentClassLogger();
				}
				return _logger;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void CheckUniqueness()
		{
			if (File.Exists(Config.RunningLockPath))
			{
				try
				{
					File.Delete(Config.RunningLockPath);
				}
				catch (Exception)
				{
					Console.WriteLine("Agent is running");
					Console.WriteLine("Enter any key to exit:");
					Environment.Exit(1);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void CheckConfig()
		{
			string nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
			if (!File.Exists(nlogConfigPath))
			{
				Console.WriteLine("NLog configuraiton unfound.");
				Console.WriteLine("Enter any key to exit:");
				Environment.Exit(1);
			}
			_logger = LogManager.GetCurrentClassLogger();
			string configPath = Path.Combine(AppContext.BaseDirectory, "config.ini");
			if (!File.Exists(configPath))
			{
				_logger.Error("config.ini unfound.");
				Console.WriteLine("Enter any key to exit:");
				Environment.Exit(1);
			}

			Logger.Info($"[{++step}] Check configuration exists success.");
		}

		/// <summary>
		/// 
		/// </summary>
		public void LoadConfig()
		{
			string nlogConfigPath = Path.Combine(AppContext.BaseDirectory, "nlog.config");
			LogManager.Configuration = new XmlLoggingConfiguration(nlogConfigPath);

			Config.Load();

			Logger.Info($"[{++step}] Load configuration success.");
		}

		/// <summary>
		/// 
		/// </summary>
		public void Start()
		{
			while (!_exit)
			{
				Heartbeat();
				Thread.Sleep(Config.HeartbeatInterval);
			}
		}

		public void StartAysnc()
		{
			_task = Task.Factory.StartNew(() =>
			{
				Start();
			});
		}

		/// <summary>
		/// 
		/// </summary>
		public void Exit()
		{
			_logger.Info("Exiting...");

			_exit = true;
			_task?.Wait();
			while (Processes.Count > 0)
			{
				_logger.Info("Wait crawler processes exit.");
				Thread.Sleep(5000);
			}
			if (File.Exists(Config.RunningLockPath))
			{
				try
				{
					File.Delete(Config.RunningLockPath);
				}
				catch
				{
					_logger.Info("Delete process lock failed.");
				}
			}
			_logger.Info("Exit success.");
		}

		private async void Heartbeat()
		{
			try
			{
				var hearbeat = HeartBeat.Create();
				hearbeat.ProcessCount = Processes.Count;
				var json = JsonConvert.SerializeObject(hearbeat);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var url = Config.HeartbeatUrl;
				await httpClient.PostAsync(url, content).ContinueWith((task) =>
				{
					HttpResponseMessage response = task.Result;
					response.EnsureSuccessStatusCode();
					var result = response.Content.ReadAsStringAsync().Result;
					if (!string.IsNullOrEmpty(result))
					{
						var commands = JsonConvert.DeserializeObject<Messsage[]>(result);
						foreach (var command in commands)
						{
							Excecute(command); ;
						}
					}
				});

				_logger.Trace(hearbeat);
			}
			catch (Exception e)
			{
				_logger.Error($"Heartbeart failed: {e}");
			}
		}

		private void Excecute(Messsage command)
		{
			switch (command.Name)
			{
				case Messsage.RunName:
					{
						lock (this)
						{
							Run(command);
						}
						break;
					}
				case Messsage.CancelName:
					{
						lock (this)
						{
							Canle(command);
						}
						break;
					}
				case Messsage.ExitName:
					{
						lock (this)
						{
							Exit();
						}
						break;
					}
			}
		}

		private void Canle(Messsage command)
		{
			if (command.NodeId != Config.NodeId)
			{
				Logger.Error($"Pemission denied.");
				return;
			}
			if (!Processes.ContainsKey(command.TaskId))
			{
				Logger.Warn($"Task {command.TaskId} is not running");
				return;
			}

			ProcessInfo processInfo;
			if (Processes.TryGetValue(command.TaskId, out processInfo))
			{
				var closeSignal = Path.Combine(processInfo.WorkingDirectory, $"{processInfo.TaskId}_close");
				File.WriteAllText(closeSignal, "");

				processInfo.Process.WaitForExit(30000);

				try
				{
					if (!processInfo.Process.HasExited)
					{
						processInfo.Process.Kill();
					}
					if (File.Exists(closeSignal))
					{
						File.Delete(closeSignal);
					}
				}
				catch (Exception e)
				{
					_logger.Error($"Close process for task {command.TaskId} failed: {e}");
				}
			}
		}

		private void Run(Messsage command)
		{
			if (command.NodeId != Config.NodeId)
			{
				Logger.Error($"Pemission denied.");
				return;
			}
			string workingDirectory;

			if (Processes.ContainsKey(command.TaskId))
			{
				Logger.Error($"Task {command.TaskId} is already running");
				return;
			}

			if (string.IsNullOrEmpty(command.Version))
			{
				Logger.Warn($"Version {command.Version} unfound.");
				return;
			}

			var directory = Path.Combine(Config.ProjectsDirectory, command.TaskId.ToString());
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
				Logger.Info("Create task folder success.");
			}

			workingDirectory = Path.Combine(directory, command.Version);
			if (!Directory.Exists(workingDirectory))
			{
				try
				{
					var zip = Path.Combine(Config.PackagesDirectory, "{command.Version}.ZIP");
					var packageUrl = $"{Config.PackageUrl}{command.Version}.ZIP";
					var bytes = httpClient.GetByteArrayAsync(packageUrl).Result;
					File.WriteAllBytes(zip, bytes);
					ZipUtil.UnZip(zip, workingDirectory);
				}
				catch (Exception e)
				{
					Logger.Error($"Download package failed: {e}.");
					return;
				}
			}

			var process = Process(command.ApplicationName, command.Arguments, workingDirectory, () =>
			{
				ProcessInfo p;
				Processes.TryRemove(command.TaskId, out p);
			});
			ProcessInfo info = new ProcessInfo
			{
				TaskId = command.TaskId.ToString(),
				Process = process,
				WorkingDirectory = workingDirectory
			};

			Processes.TryAdd(command.TaskId, info);
		}

		public Process Process(string app, string arguments, string workingDirectory, Action onExited)
		{
			var path = Path.Combine(workingDirectory, app);
			path = File.Exists(path) ? path : app;

			Process process = new Process
			{
				StartInfo =
					{
						FileName = path,
						UseShellExecute = false,
						CreateNoWindow = false,
						WorkingDirectory = workingDirectory,
						Arguments = arguments
					},
				EnableRaisingEvents = true
			};
			process.Start();
			process.Exited += (a, b) => { onExited?.Invoke(); };
			return process;
		}
	}
}
