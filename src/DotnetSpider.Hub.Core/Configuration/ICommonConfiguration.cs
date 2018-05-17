namespace DotnetSpider.Hub.Core.Configuration
{
	public interface ICommonConfiguration
	{
		string MsSqlConnectionString { get; }
		string MySqlConnectionString { get; }
		string SchedulerUrl { get; }
		string SchedulerCallback { get; }
		string[] Tokens { get; }
		bool AuthorizeApi { get; }
		bool RecordGloabException { get; }
		string[] AgentTypes { get; }
	}
}
