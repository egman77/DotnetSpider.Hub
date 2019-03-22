namespace DotnetSpider.Hub.Core.Configuration
{
	public class CommonConfiguration : ICommonConfiguration
	{
		public string ConnectionString { get; set; }

		public string SchedulerUrl { get; set; }
        /// <summary>
        /// 计划回调
        /// </summary>
		public string SchedulerCallback { get; set; }

		public string[] Tokens { get; set; }

		public string[] NodeTypes { get; set; }

		public bool RequireToken { get; set; }

        /// <summary>
        /// 是否忽略计划任务
        /// </summary>
		public string IngoreCron { get; set; }
	}
}
