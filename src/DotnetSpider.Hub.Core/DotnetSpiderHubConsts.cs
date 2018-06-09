namespace DotnetSpider.Hub.Core
{
	public class DotnetSpiderHubConsts
	{
		public const string Settings = "DotnetSpiderHub";
		public const string DotnetSpiderHub = "DotnetSpiderHub";

		public const string UrlPattern = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-,#]*)?(\?[a-zA-Z0-9\&%_\./-~-,#]*)?";

		public const string SchedulerUrl = "SchedulerUrl";
		public const string SchedulerCallback = "SchedulerCallback";
		public const string GloabExceptionLog = "GloabExceptionLog";
		public const string Tokens = "Tokens";
		public const string RequireToken = "RequireToken";
		public const string NodeTypes = "NodeTypes";
		public const string IngoreCron = "IngoreCron";
		public const string JobPrefix = "DOTNETSPIDER.HUB.";
	}
}
