using System;
using System.Threading;
using DotnetSpider.Hub.Agent.Process;
using Serilog;

namespace DotnetSpider.Hub.Agent.Command
{
	public class Exit : Command
	{
		public override string Name => CommandNames.ExitName;

		public override void Execute(Messsage command, AgentClient client)
		{
			Log.Logger.Information("Waiting all exists crawler processes...");

			while (ProcessManager.ProcessCount > 0)
			{
				Thread.Sleep(1000);
			}
			Log.Logger.Information("All exists crawler processes exit success.");
			client.Dispose();
			Environment.Exit(0);
		}
	}
}
