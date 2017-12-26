using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace DotnetSpider.Enterprise.Agent
{
	public static class CommandExecutor
	{
		private readonly static ILogger Logger = LogManager.GetCurrentClassLogger();
		private static readonly ConcurrentDictionary<long, ProcessDetail> Processes = new ConcurrentDictionary<long, ProcessDetail>();
		private static readonly object ExecuteLock = new object();

		public static int ProcessCount => Processes.Count;

		public static event Action OnExited;

		public static void Execute(Messsage command)
		{
			if (command.NodeId != Config.NodeId)
			{
				Logger.Error($"Command error: {JsonConvert.SerializeObject(command)}.");
				return;
			}

			Logger.Info($"Consume message: {JsonConvert.SerializeObject(command)}.");
			lock (ExecuteLock)
			{
				try
				{
					switch (command.Name)
					{
						case Messsage.RunName:
							{
								Run(command);
								break;
							}
						case Messsage.CancelName:
							{
								Cancel(command);
								break;
							}
						case Messsage.ExitName:
							{
								Exit();
								break;
							}
					}
				}
				catch (Exception e)
				{
					Logger.Error($"Execute command {JsonConvert.SerializeObject(command)} failed: {e}");
				}
			}
		}

		public static void Exit()
		{
			Logger.Info("Waiting all exists crawler processes...");

			while (ProcessCount > 0)
			{
				Thread.Sleep(5000);
			}
			Logger.Info("All exists crawler processes exit success.");
			OnExited?.Invoke();
		}

		private static void Cancel(Messsage command)
		{
			if (!Processes.ContainsKey(command.TaskId))
			{
				Logger.Warn($"Task {command.TaskId} is not running.");
				return;
			}

			ProcessDetail processInfo;
			if (Processes.TryGetValue(command.TaskId, out processInfo))
			{
				var process = processInfo.Process;
				try
				{
					SendExitSignal(command.TaskId.ToString(), processInfo.WorkingDirectory);
				}
				catch
				{
					//ignore
				}
				process.WaitForExit(30000);

				try
				{
					process.Kill();
				}
				catch (NotSupportedException nse)
				{
					Logger.Info($"Kill task {command.TaskId} success: {nse.Message}.");
				}
				catch (Win32Exception we)
				{
					Logger.Info($"Kill task {command.TaskId} success: {we.Message}.");
				}
				catch (InvalidOperationException ioe)
				{
					Logger.Info($"Kill task {command.TaskId} success: {ioe.Message}.");
				}
				catch (Exception e)
				{
					Logger.Error($"Kill task {command.TaskId} failed: {e}.");
				}
			}
		}

		private static void Run(Messsage command)
		{
			if (Processes.ContainsKey(command.TaskId))
			{
				Logger.Error($"Task {command.TaskId} is already running.");
				return;
			}

			if (string.IsNullOrEmpty(command.Version) || string.IsNullOrWhiteSpace(command.Version))
			{
				Logger.Error($"Version should not be empty.");
				return;
			}

			var taskDirectory = Path.Combine(Config.ProjectsDirectory, command.TaskId.ToString());
			if (!Directory.Exists(taskDirectory))
			{
				Directory.CreateDirectory(taskDirectory);
				Logger.Info($"Create task directory {taskDirectory} success.");
			}

			string workingDirectory = Path.Combine(taskDirectory, command.Version);
			if (!Directory.Exists(workingDirectory))
			{
				var packageUrl = $"{Config.PackageUrl}{command.Version}.zip";
				try
				{
					var localPackageFilePath = Path.Combine(Config.PackagesDirectory, $"{command.Version}.zip");
					var bytes = Config.HttpClient.GetByteArrayAsync(packageUrl).Result;
					File.WriteAllBytes(localPackageFilePath, bytes);
					ZipFile.ExtractToDirectory(localPackageFilePath, workingDirectory);
				}
				catch (Exception e)
				{
					Logger.Error($"Download package {packageUrl} failed: {e}.");
					return;
				}
			}

			var process = StartProcess(command.ApplicationName, command.Arguments, workingDirectory, () =>
			{
				Logger.Info($"Process of task {command.TaskId} exited.");
				ProcessDetail p;
				Processes.TryRemove(command.TaskId, out p);
			});

			Processes.TryAdd(command.TaskId, new ProcessDetail
			{
				TaskId = command.TaskId.ToString(),
				Process = process,
				WorkingDirectory = workingDirectory
			});
		}

		private static Process StartProcess(string app, string arguments, string workingDirectory, Action onExited)
		{
			var path = Path.Combine(workingDirectory, app);
			path = File.Exists(path) ? path : app;

			Process process = new Process
			{
				StartInfo =
				{
					FileName = path,
					UseShellExecute = true,
					CreateNoWindow = true,
					WorkingDirectory = workingDirectory,
					Arguments = arguments
				},
				EnableRaisingEvents = true
			};
			process.Start();
			process.Exited += (a, b) => { onExited?.Invoke(); };
			return process;
		}

		private static void SendExitSignal(string taskId, string workdirectory)
		{
			if (Config.IsRunningOnWindows)
			{
				var taskIdMmf = MemoryMappedFile.OpenExisting(taskId, MemoryMappedFileRights.Write);
				if (taskIdMmf != null)
				{
					using (MemoryMappedViewStream stream = taskIdMmf.CreateViewStream())
					{
						var writer = new BinaryWriter(stream);
						writer.Write(1);
					}
				}
			}
			else
			{
				File.Create(Path.Combine(workdirectory, $"{taskId}_cl"));
			}
		}
	}
}
