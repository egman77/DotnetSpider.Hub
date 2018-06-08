namespace DotnetSpider.Hub.Core.Configuration
{
	public interface ICommonConfiguration
	{
		string ConnectionString { get; }
		string SchedulerUrl { get; }
		string SchedulerCallback { get; }
		string[] Tokens { get; }
		bool RequireToken { get; }
		string[] NodeTypes { get; }
	}
}
