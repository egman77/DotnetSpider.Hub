using NLog;

namespace DotnetSpider.Enterprise.Agent.Command
{
	public abstract class Command : ICommand
	{
		protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		public abstract string Name { get; }

		public abstract void Execute(Messsage command, AgentClient client); 
	}
}
