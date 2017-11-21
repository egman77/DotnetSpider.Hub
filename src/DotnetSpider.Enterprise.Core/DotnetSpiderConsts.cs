using System;

namespace DotnetSpider.Enterprise.Core
{
	public class DotnetSpiderConsts
	{
		public const string LocalizationSourceName = "DotnetSpider.Enterprise";
		public const string DefaultSetting = "DotnetSpider.Enterprise";
		public const string ConnectionName = "DefaultConnection";
		public const string LogMongoConnectionName = "LogMongoConnection";

		public const string EmailSenderHost = "email.sender.host";
		public const string EmailSenderPort = "email.sender.port";
		public const string EmailSenderPassword = "email.sender.password";
		public const string EmailSenderEnableSsl = "email.sender.enableSsl";
		public const string EmailSenderFromDisplayName = "email.sender.fromDisplayName";
		public const string EmailSenderFromAddress = "email.sender.fromAddress";

		public const string UrlRegexPattern = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-,#]*)?(\?[a-zA-Z0-9\&%_\./-~-,#]*)?";

		public const string XmlKeyPath = "xml.key.path";

		public const string SmsApi = "sms.api";

		public const string SchedulerUrl = "scheduler.url";
		public const string SchedulerCallbackHost = "scheduler.callbackHost";
	}
}
