namespace DotnetSpider.Hub.Core
{
	public class DotnetSpiderConsts
	{
		public const string Settings = "DotnetSpiderHub";
		public const string DefaultConnection = "DefaultConnection";
		public const string MySqlConnection = "MySqlConnection";

		public const string UrlRegexPattern = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-,#]*)?(\?[a-zA-Z0-9\&%_\./-~-,#]*)?";

		public const string SchedulerUrl = "SchedulerUrl";
		public const string SchedulerCallback = "SchedulerCallback";
		public const string AuthorizeApi = "AuthorizeApi";
		public const string RecordGloabException = "RecordGloabException";
		public const string Tokens = "Tokens";
		public const string AgentTypes = "AgentTypes";
		public const string UnTriggerCron = "* * * * 2999";
		public const string SystemJobPrefix = "System.DotnetSpider.";
	}
}
