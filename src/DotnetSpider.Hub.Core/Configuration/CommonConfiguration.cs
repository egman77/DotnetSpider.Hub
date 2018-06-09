namespace DotnetSpider.Hub.Core.Configuration
{
	public class CommonConfiguration : ICommonConfiguration
	{
		public string ConnectionString { get; set; }

		public string SchedulerUrl { get; set; }

		public string SchedulerCallback { get; set; }

		public string[] Tokens { get; set; }

		public string[] NodeTypes { get; set; }

		public bool RequireToken { get; set; }

		public string IngoreCron { get; set; }
	}
}
