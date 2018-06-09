using System.Collections.Concurrent;
using System.IO;
using NLog;

namespace DotnetSpider.Hub.Agent.Process
{
	public class ProcessManager
	{
		protected readonly static ILogger Logger = LogManager.GetCurrentClassLogger();
		private static readonly ConcurrentDictionary<string, ProcessInfo> Processes = new ConcurrentDictionary<string, ProcessInfo>();

		public static int ProcessCount => Processes.Count;

		public static bool IsTaskExsits(string taskId)
		{
			return Processes.ContainsKey(taskId);
		}

		public static void StartProcess(string taskId, string app, string arguments, string workingDirectory)
		{
			Logger.Info($"Start process for task: {taskId}, app: {app}, arguments: {arguments}, workingDirectory: {workingDirectory}.");
			var path = Path.Combine(workingDirectory, app);
			path = File.Exists(path) ? path : app;
			System.Diagnostics.Process process = new System.Diagnostics.Process
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
			Logger.Info($"Start process for task: {taskId} success.");
			process.Exited += (a, b) =>
			{
				Logger.Info($"Process of task {taskId} exited.");
				ProcessInfo p;
				Processes.TryRemove(taskId, out p);
			};
			Processes.TryAdd(taskId, new ProcessInfo
			{
				TaskId = taskId.ToString(),
				Process = process,
				WorkingDirectory = workingDirectory
			});
		}

		public static ProcessInfo GetProcessDetail(string taskId)
		{
			ProcessInfo processDetail;
			if (Processes.TryGetValue(taskId, out processDetail))
			{
				return processDetail;
			}
			return null;
		}
	}
}
