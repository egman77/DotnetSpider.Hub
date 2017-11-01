using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DotnetSpider.Enterprise.Application
{
	public abstract class AppServiceBase
	{
		//protected static readonly AppRedisDb TokenDb;
		//protected static readonly AppRedisDb ConnectionDb;
		//protected static readonly AppRedisDb RunningDb;
		//protected static readonly AppRedisDb ScheduleTasksDb;
		//protected static readonly ISubscriber Subscriber;

		//protected static readonly IDatabase ServerAgentsDb;
		//protected static readonly AppRedisDb TaskEndingDb;
		//protected static readonly AppRedisDb SiteToolTokenDb;
		//protected static readonly AppRedisDb AgentNodesDb;

		protected readonly ICommonConfiguration Configuration;
		protected readonly UserManager<Domain.Entities.ApplicationUser> UserManager;
		protected readonly ApplicationDbContext DbContext;

		protected IAppSession Session { get; }

		static AppServiceBase()
		{
			ICommonConfiguration configuration = DI.IocManager.GetRequiredService<ICommonConfiguration>();

			var conn = ConnectionMultiplexer.Connect(new ConfigurationOptions()
			{
				ServiceName = "Token",
				Password = configuration.RedisPassword,
				ConnectTimeout = 5000,
				KeepAlive = 8,
				EndPoints =
				{
					{ configuration.RedisHost,configuration.RedisPort }
				}
			});
			var db = conn.GetDatabase(configuration.RedisDb);
			//TokenDb = new AppRedisDb(configuration.RedisTokenNamespace, db);
			//ConnectionDb = new AppRedisDb(configuration.RedisConnectionNamespace, db);
			//RunningDb = new AppRedisDb(configuration.RedisRunningTaskNamespace, db);
			//ScheduleTasksDb = new AppRedisDb(configuration.RedisScheduleTasksNamespace, db);
			//TaskEndingDb = new AppRedisDb(configuration.RedisTaskEndingNamespace, db);
			//Subscriber = conn.GetSubscriber();
			//ServerAgentsDb = conn.GetDatabase(configuration.RedisNodeAliveDb);
			//SiteToolTokenDb = new AppRedisDb(configuration.RedisSiteToolTokenNamespace, db);
			//AgentNodesDb = new AppRedisDb(configuration.RedisAgentNodesNamespace, db);
		}

		protected AppServiceBase(ApplicationDbContext dbcontext)
		{
			Session = DI.IocManager.GetRequiredService<IAppSession>();
			DbContext = dbcontext;  //DI.IocManager.GetRequiredService<ApplicationDbContext>();

			Configuration = DI.IocManager.GetRequiredService<ICommonConfiguration>();
			UserManager = DI.IocManager.GetRequiredService<UserManager<Domain.Entities.ApplicationUser>>();
		}

		protected virtual Domain.Entities.ApplicationUser GetCurrentUser()
		{
			var userId = Session.UserId.Value;
			var user = UserManager.GetUserById(userId);
			if (user == null)
			{
				throw new AppException("There is no current user!");
			}

			return user;
		}

		protected virtual string GetClientIp()
		{
			var httpContextAccessor = DI.IocManager.GetRequiredService<IHttpContextAccessor>();
			var ip = httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"];

			if (string.IsNullOrEmpty(ip))
			{
				ip = httpContextAccessor.HttpContext.Request.Headers["server.RemoteIpAddress"];
			}

			if (string.IsNullOrEmpty(ip))
			{
				throw new AppException("Cannot Detect Client Ip");
			}
			return ip;
		}

		protected bool CheckMyPermission(string claimName, bool throwException = true)
		{
			var r = UserManager.HasClaim(claimName);
			if (throwException)
			{
				if (!r)
				{
					throw new AppException($"Permission \"{claimName}\" required. Please contact Pa1Pa Service.");
				}
			}
			return r;
		}

		//private long GetDataSize(long userId)
		//{
		//	var dbName = $"db_{Encrypt.Md5Encrypt(userId.ToString())}";
		//	var conn = DbContext.Database.GetDbConnection();
		//	DbContext.Database.OpenConnection();
		//	var cmd = conn.CreateCommand();
		//	cmd.CommandText = $"SELECT SUM(((size* 8) / 1024)) FROM sys.master_files WHERE DB_NAME(database_id) IN('{dbName}')";
		//	cmd.CommandType = System.Data.CommandType.Text;
		//	var value = cmd.ExecuteScalar();
		//	if (value is DBNull)
		//	{
		//		return 0;
		//	}
		//	else
		//	{
		//		return Convert.ToInt64(value);
		//	}
		//}
	}
}
