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
		private static readonly ConcurrentDictionary<string, Process> Processes = new ConcurrentDictionary<string, Process>();
		private static IConfigurationRoot _configurationRoot;
		private static ILogger _logger;
		private readonly HttpClient httpClient = new HttpClient();
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
			if (File.Exists(AgentConsts.RunningLockPath))
			{
				try
				{
					File.Delete(AgentConsts.RunningLockPath);
				}
				catch (Exception)
				{
					Console.WriteLine("Agent is running");
					Console.WriteLine("Enter any key to exit:");
					Console.Read();
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
				Console.Read();
				Environment.Exit(1);
			}
			_logger = LogManager.GetCurrentClassLogger();
			string configPath = Path.Combine(AppContext.BaseDirectory, "config.ini");
			if (!File.Exists(configPath))
			{
				_logger.Error("config.ini unfound.");
				Console.WriteLine("Enter any key to exit:");
				Console.Read();
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

			var builder = new ConfigurationBuilder();
			builder.AddIniFile("config.ini");

			_configurationRoot = builder.Build();

			Config.Load(_configurationRoot);

			AgentConsts.Load();

			Logger.Info($"[{++step}] Load configuration success.");
		}

		/// <summary>
		/// 
		/// </summary>
		public void Start()
		{
			_task = Task.Factory.StartNew(() =>
			{
				while (!_exit)
				{
					Heartbeat();
					Thread.Sleep(Config.Instance.HeartbeatInterval);
				}
			});
		}

		/// <summary>
		/// 
		/// </summary>
		public void Exit()
		{
			_logger.Info("Exiting...");

			_exit = true;
			_task.Wait();
			while (Processes.Count > 0)
			{
				_logger.Info("Wait crawler processes exit.");
				Thread.Sleep(5000);
			}
			if (File.Exists(AgentConsts.RunningLockPath))
			{
				try
				{
					File.Delete(AgentConsts.RunningLockPath);
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
			var hearbeat = HeartBeat.Create();
			hearbeat.CountOfProcess = Processes.Count;
			var json = JsonConvert.SerializeObject(hearbeat);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var url = Config.Instance.HeartbeatUrl;
				await httpClient.PostAsync(url, content).ContinueWith((task) =>
				{
					HttpResponseMessage response = task.Result;
					response.EnsureSuccessStatusCode();

					try
					{
						var result = response.Content.ReadAsStringAsync().Result;
						if (!string.IsNullOrEmpty(result))
						{
							var commands = JsonConvert.DeserializeObject<Command[]>(result);
							foreach (var command in commands)
							{
								Excecute(command); ;
							}
						}
					}
					catch (Exception e)
					{
						_logger.Error(e, e.ToString());
					}
				});
			}
			catch (Exception e)
			{
				_logger.Error($"Send heartbeart failed.: {e}");
			}
		}

		private void Excecute(Command command)
		{
			switch (command.Name)
			{
				case Command.RunName:
					{
						Task.Factory.StartNew(() => { Run(command); });
						break;
					}
				case Command.CanleName:
					{
						Canle(command);
						break;
					}
			}
		}

		private void Canle(Command command)
		{
			if (!Processes.ContainsKey(command.Task))
			{
				Logger.Warn($"Task {command.Task} is not running");
				return;
			}
			if (command.AngentId != AgentConsts.AgentId)
			{
				Logger.Error($"Pemission denied.");
				return;
			}
			Process process;
			if (Processes.TryGetValue(command.Task, out process))
			{
				process.Close();
				process.WaitForExit(60000);
				if (!process.HasExited)
				{
					process.Kill();
				}
			}
		}


		private void Run(Command command)
		{
			if (Processes.ContainsKey(command.Task))
			{
				Logger.Error($"Task {command.Task} is already running");
				return;
			}

			if (string.IsNullOrEmpty(command.Version))
			{
				Logger.Warn($"Version unfound.");
				return;
			}

			var directory = Path.Combine(AgentConsts.ProjectsDirectory, command.Task);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
				Logger.Info("Create task folder success.");
			}

			var workingDirectory = Path.Combine(directory, command.Version);
			if (!Directory.Exists(workingDirectory))
			{
				try
				{
					var zip = Path.Combine(AgentConsts.PackagesDirectory, "{command.Version}.ZIP");
					var bytes = httpClient.GetByteArrayAsync($"{Config.Instance.PackageUrl}/{command.Version}.ZIP").Result;
					File.WriteAllBytes(zip, bytes);
					ZipUtil.UnZip(zip, workingDirectory);
				}
				catch (Exception e)
				{
					Logger.Error($"Download package failed: {e}.");
					return;
				}
			}


			var process = Process(command.Application, command.Arguments, workingDirectory, () =>
			{
				Process p;
				Processes.TryRemove(command.Task, out p);
			});
			Processes.TryAdd(command.Task, process);
		}

		public static Process Process(string app, string arguments, string workingDirectory, Action onExited)
		{
			Process process = new Process
			{
				StartInfo =
					{
						FileName = app,
						UseShellExecute = false,
						CreateNoWindow = false,
						WorkingDirectory = workingDirectory,
						Arguments = arguments
					}
			};

			process.Start();
			process.Exited += (a, b) => { onExited?.Invoke(); };
			return process;
		}
	}
}
