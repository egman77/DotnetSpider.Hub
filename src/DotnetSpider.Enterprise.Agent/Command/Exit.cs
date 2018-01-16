using DotnetSpider.Enterprise.Agent.Process;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DotnetSpider.Enterprise.Agent.Command
{
	public class Exit : Command
	{
		public override string Name => CommandNames.ExitName;

		public override void Execute(Messsage command, AgentClient client)
		{
			Logger.Info("Waiting all exists crawler processes...");

			while (ProcessManager.ProcessCount > 0)
			{
				Thread.Sleep(1000);
			}
			Logger.Info("All exists crawler processes exit success.");
			client.Dispose();
		}
	}
}
