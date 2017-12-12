using System;

namespace DotnetSpider.Enterprise.Core
{
	public class DotnetSpiderConsts
	{
		public const string LocalizationSourceName = "DotnetSpider.Enterprise";
		public const string DefaultSetting = "DotnetSpider.Enterprise";
		public const string ConnectionName = "DefaultConnection";
		public const string MySqlConnectionName = "MySqlConnection";
		public const string LogMongoConnectionName = "LogMongoConnection";

		public const string UrlRegexPattern = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-,#]*)?(\?[a-zA-Z0-9\&%_\./-~-,#]*)?";

		public const string SchedulerUrl = "scheduler.url";
		public const string SchedulerCallbackHost = "scheduler.callbackHost";
		public const string Tokens = "tokens";
		public const string UnTriggerCron = "* * * * 2999";
		public const string SqlEncryptCode = "sqlEncryptCode";
		public const string SystemJobPrefix = "System.DotnetSpider.";
	}
}
