using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotnetSpider.Enterprise.Agent.Process
{
	public class ProcessManager
	{
		protected readonly static ILogger Logger = LogManager.GetCurrentClassLogger();
		private static readonly ConcurrentDictionary<long, ProcessDetail> Processes = new ConcurrentDictionary<long, ProcessDetail>();

		public static int ProcessCount => Processes.Count;

		public static bool IsTaskExsits(long taskId)
		{
			return Processes.ContainsKey(taskId);
		}

		public static void StartProcess(long taskId, string app, string arguments, string workingDirectory)
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
				ProcessDetail p;
				Processes.TryRemove(taskId, out p);
			};
			Processes.TryAdd(taskId, new ProcessDetail
			{
				TaskId = taskId.ToString(),
				Process = process,
				WorkingDirectory = workingDirectory
			});
		}

		public static ProcessDetail GetProcessDetail(long taskId)
		{
			ProcessDetail processDetail;
			if (Processes.TryGetValue(taskId, out processDetail))
			{
				return processDetail;
			}
			return null;
		}
	}
}
