namespace DotnetSpider.Hub.Core.Configuration
{
	public class CommonConfiguration : ICommonConfiguration
	{
		public string MsSqlConnectionString { get; set; }

		public string MySqlConnectionString { get; set; }

		public string SchedulerUrl { get; set; }

		public string SchedulerCallback { get; set; }

		public string[] Tokens { get; set; }

		public string[] AgentTypes { get; set; }

		public bool AuthorizeApi { get; set; }

		public bool RecordGloabException { get; set; }
	}
}
