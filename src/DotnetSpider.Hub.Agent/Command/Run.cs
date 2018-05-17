using System;
using System.IO;
using System.IO.Compression;
using DotnetSpider.Hub.Agent.Process;

namespace DotnetSpider.Hub.Agent.Command
{
	public class Run : Command
	{
		public override string Name => CommandNames.RunName;

		public override void Execute(Messsage command, AgentClient client)
		{
			if (ProcessManager.IsTaskExsits(command.TaskId))
			{
				Logger.Error($"Task {command.TaskId} is already running.");
				return;
			}

			if (string.IsNullOrEmpty(command.Version) || string.IsNullOrWhiteSpace(command.Version))
			{
				Logger.Error($"Version should not be empty.");
				return;
			}
			Logger.Info($"Start prepare workdirectory...");
			var taskDirectory = Path.Combine(Env.ProjectsDirectory, command.TaskId.ToString());
			if (!Directory.Exists(taskDirectory))
			{
				Directory.CreateDirectory(taskDirectory);
				Logger.Info($"Create task directory {taskDirectory} success.");
			}

			string workingDirectory = Path.Combine(taskDirectory, command.Version);
			if (!Directory.Exists(workingDirectory))
			{
				var packageUrl = $"{Env.PackageUrl}{command.Version}.zip";
				try
				{
					var localPackageFilePath = Path.Combine(Env.PackagesDirectory, $"{command.Version}.zip");
					var bytes = Env.HttpClient.GetByteArrayAsync(packageUrl).Result;
					File.WriteAllBytes(localPackageFilePath, bytes);
					ZipFile.ExtractToDirectory(localPackageFilePath, workingDirectory);
				}
				catch (Exception e)
				{
					Logger.Error($"Download package {packageUrl} failed: {e}.");
					return;
				}
			}
			ProcessManager.StartProcess(command.TaskId, command.ApplicationName, command.Arguments, workingDirectory);
		}
	}
}
