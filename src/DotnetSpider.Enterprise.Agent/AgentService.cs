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
		private static readonly ConcurrentDictionary<long, Process> Processes = new ConcurrentDictionary<long, Process>();
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

			Config.Load();

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
					Thread.Sleep(Config.HeartbeatInterval);
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
			var hearbeat = HeartBeat.Create();
			hearbeat.ProcessCount = Processes.Count;
			var json = JsonConvert.SerializeObject(hearbeat);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var url = Config.HeartbeatUrl;
				await httpClient.PostAsync(url, content).ContinueWith((task) =>
				{
					HttpResponseMessage response = task.Result;
					response.EnsureSuccessStatusCode();

					try
					{
						var result = response.Content.ReadAsStringAsync().Result;
						if (!string.IsNullOrEmpty(result))
						{
							var commands = JsonConvert.DeserializeObject<Messsage[]>(result);
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
				case Messsage.CanleName:
					{
						lock (this)
						{
							Canle(command);
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

			Process process;
			if (Processes.TryGetValue(command.TaskId, out process))
			{
				process.Close();
				process.WaitForExit(60000);
				if (!process.HasExited)
				{
					process.Kill();
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
				Logger.Warn($"Version unfound.");
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
					var bytes = httpClient.GetByteArrayAsync($"{Config.PackageUrl}/{command.Version}.ZIP").Result;
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
				Process p;
				Processes.TryRemove(command.TaskId, out p);
				ReportProcessCountChanged(command.TaskId, false);
			});

			Processes.TryAdd(command.TaskId, process);

			ReportProcessCountChanged(command.TaskId, true);
		}

		public Process Process(string app, string arguments, string workingDirectory, Action onExited)
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
					},
				EnableRaisingEvents = true
			};

			process.Start();
			process.Exited += (a, b) => { onExited?.Invoke(); };
			return process;
		}

		private void ReportProcessCountChanged(long taskId, bool isStart)
		{
			var content = new StringContent($"{{'token':'{Config.ApiToken}','taskId':'{taskId}','isStart':'{isStart}',}}", Encoding.UTF8, "application/json");

			for (int i = 0; i < 100; ++i)
			{
				try
				{
					var url = Config.ProcessCountChangedUrl;
					httpClient.PostAsync(url, content).ContinueWith((task) =>
					{
						HttpResponseMessage response = task.Result;
						response.EnsureSuccessStatusCode();
					}).Wait();
					break;
				}
				catch (Exception e)
				{
					_logger.Error($"ReportProcessCountChanged failed.: {e}");
					Thread.Sleep(6000);
				}
			}
		}
	}
}
