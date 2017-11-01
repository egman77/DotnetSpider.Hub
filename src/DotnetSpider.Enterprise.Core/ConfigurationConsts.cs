using System;

namespace DotnetSpider.Enterprise.Core
{
	public class ConfigurationConsts
	{
		public const string LocalizationSourceName = "DotnetSpider.Enterprise";
		public const string DefaultSetting = "DotnetSpider.Enterprise";
		public const string DefaultTenantIdSetting = "Abp.Tenant.DefaultId";
		public const string ConnectionStringName = "DefaultConnection";
		public const string MySqlConnectionStringName = "MysqlConnection";
		public const string DataSizeCollectionName = "data_size";
		public const string DataHubName = "websocket.latestHub";
		public const string RedisDb = "redis.db";

		public const string RedisNamespace = "redis.namespace";

		public const string RedisQuartzSync = "quartz.sync";
		public const string RedisHost = "redis.host";
		public const string RedisPort = "redis.port";
		public const string RedisPassword = "redis.password";
		public const string ExtensionDownloadCount = "ExtensionDownloadCount";
		public const string PageSize = "page.size";
		public const string PageMaxSize = "page.max.size";
		public const string PageMaxNumber = "page.max.number";
		public const string TenantMaxUser = "tenant.max.user";

		public const string DefaultFreeStorage = "default.free.storage";
		public const string DefaultAdvancedStorage = "default.advanced.storage";
		public const string DefaultEnterpriseStorage = "default.enterprise.storage";

		public const string DefaultMonthlyStorage = "default.monthly.storage";
		public const string PriceAdvancedPerMonth = "price.advanced.per.month";
		public const string PricePerG = "price.per.g";
		public const string PriceNormalPerMonth = "price.normal.per.month";

		public const string DefaultCoupon = "default.coupon";
		public const string DefaultRewardCoupon = "default.reward.coupon";

		public const string EmailSenderHost = "email.sender.host";
		public const string EmailSenderPort = "email.sender.port";
		public const string EmailSenderPassword = "email.sender.password";
		public const string EmailSenderEnableSsl = "email.sender.enableSsl";
		public const string EmailSenderFromDisplayName = "email.sender.fromDisplayName";
		public const string EmailSenderFromAddress = "email.sender.fromAddress";
		public const string DownloadMaxRows = "download.maxRows";

		public const string UrlRegexPattern = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-,#]*)?(\?[a-zA-Z0-9\&%_\./-~-,#]*)?";

		public const string XmlKeyPath = "xml.key.path";

		public const string ExtensionVersion = "version";
		public const string ExtensionBackgroundVersion = "version";
		public const string TaskNameMaxLength = "taskName.maxLength";
		public const string TaskNameMinLength = "taskName.minLength";
		public const string DownloadCenter = "downloadCenter";
		public const string IsDownloadCenter = "IsDownloadCenter";
		public const string TaskFreeStartUrlsMaxCount = "task.free.startUrls.maxCount";
		public const string TaskAdvancedStartUrlsMaxCount = "task.advanced.startUrls.maxCount";
		public const string SendQuartzErrorMail = "sendQuartzErrorMail";

		public const string SchedulerMinInterval = "scheduler.min.interval";

		public const string EnvName = "Env.Name";

		public const string DevEnvName = "Dev";
		public const string TestEnvName = "Test";
		public const string ProductEnvName = "Product";
		public const string SmsApi = "sms.api";
		public const string Ok = "OK";


		public const string RegisterPhoneCodeKey = "RegisterPhoneCode";
		public const string DefaultPhoneCodeKey = "DefaultPhoneCode";

		public const int TaskTriggerTimeout = 120;
		public const string CultureCookieName = "__DotnetSpider";
	}
}
