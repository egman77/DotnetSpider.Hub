namespace DotnetSpider.Hub.Core.Configuration
{
	public interface ICommonConfiguration
	{
		string ConnectionString { get; }
		string SchedulerUrl { get; }
        /// <summary>
        /// 计划回调
        /// </summary>
		string SchedulerCallback { get; }
		string[] Tokens { get; }
		bool RequireToken { get; }
		string[] NodeTypes { get; }
        /// <summary>
        /// 忽略计划任务
        /// </summary>
		string IngoreCron { get; }
	}
}
