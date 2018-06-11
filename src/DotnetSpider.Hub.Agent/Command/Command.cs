
namespace DotnetSpider.Hub.Agent.Command
{
	public abstract class Command : ICommand
	{
		public abstract string Name { get; }

		public abstract void Execute(Messsage command, AgentClient client); 
	}
}
