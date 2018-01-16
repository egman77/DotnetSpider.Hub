namespace DotnetSpider.Enterprise.Agent.Command
{
	public interface ICommand
	{
		string Name { get; }
		void Execute(Messsage command, AgentClient client);
	}
}
