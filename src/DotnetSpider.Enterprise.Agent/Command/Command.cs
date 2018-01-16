using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Agent.Command
{
	public abstract class Command : ICommand
	{
		protected readonly static ILogger Logger = LogManager.GetCurrentClassLogger();

		public abstract string Name { get; }

		public abstract void Execute(Messsage command, AgentClient client); 
	}
}
