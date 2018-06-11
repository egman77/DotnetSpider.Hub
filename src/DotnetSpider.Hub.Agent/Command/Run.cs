using System;
using System.IO;
using System.IO.Compression;
using DotnetSpider.Hub.Agent.Process;
using Newtonsoft.Json;
using Serilog;

namespace DotnetSpider.Hub.Agent.Command
{
	public class Run : Command
	{
		public override string Name => CommandNames.RunName;

		public override void Execute(Messsage command, AgentClient client)
		{
			if (string.IsNullOrWhiteSpace(command.Package))
			{
				Log.Logger.Error($"Package should not be empty.");
				return;
			}
			if (!command.Package.ToLower().EndsWith(".zip"))
			{
				Log.Logger.Error($"Package must be a zip.");
				return;
			}
			if (ProcessManager.IsTaskExsits(command.TaskId))
			{
				Log.Logger.Error($"Task {command.TaskId} is already running.");
				return;
			}
			Log.Logger.Information($"Start prepare workdirectory...");
			var taskDirectory = Path.Combine(Env.ProjectsDirectory, command.TaskId.ToString());
			if (!Directory.Exists(taskDirectory))
			{
				Directory.CreateDirectory(taskDirectory);
				Log.Logger.Information($"Create task directory {taskDirectory} success.");
			}
			var packageName = Path.GetFileNameWithoutExtension(command.Package);
			string workingDirectory = Path.Combine(taskDirectory, packageName);

			if (!Directory.Exists(workingDirectory))
			{
				var localPackageFilePath = Path.Combine(Env.PackagesDirectory, Path.GetFileName(command.Package));
				var bytes = Env.HttpClient.GetByteArrayAsync(command.Package).Result;
				File.WriteAllBytes(localPackageFilePath, bytes);
				ZipFile.ExtractToDirectory(localPackageFilePath, workingDirectory);

			}
			ProcessManager.StartProcess(command.TaskId, command.ApplicationName, command.Arguments, workingDirectory);

		}
	}
}
