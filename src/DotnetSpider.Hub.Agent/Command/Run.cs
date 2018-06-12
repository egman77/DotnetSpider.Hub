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
			if (!command.Package.ToLower().Contains(".zip"))
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
			Uri uri;
			if (Uri.TryCreate(command.Package, UriKind.RelativeOrAbsolute, out uri))
			{
				var packageName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
				string workingDirectory = Path.Combine(taskDirectory, packageName);

				if (!Directory.Exists(workingDirectory))
				{
					var localPackageFilePath = Path.Combine(Env.PackagesDirectory, Path.GetFileName(uri.AbsolutePath));
					var bytes = Env.HttpClient.GetByteArrayAsync(uri).Result;
					File.WriteAllBytes(localPackageFilePath, bytes);
					ZipFile.ExtractToDirectory(localPackageFilePath, workingDirectory);

				}
				ProcessManager.StartProcess(command.TaskId, command.ApplicationName, command.Arguments, workingDirectory);
			}
			else
			{
				Log.Logger.Error("Package is not a correct url.");
			}
		}
	}
}
