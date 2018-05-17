namespace DotnetSpider.Hub.Agent.Command
{
	public interface ICommand
	{
		string Name { get; }
		void Execute(Messsage command, AgentClient client);
	}
}
