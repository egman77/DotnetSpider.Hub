using DotnetSpider.Enterprise.Application.Hangfire.Dtos;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Hangfire
{
	public class HangfireAppService : AppServiceBase, IHangfireAppService
	{
		private RetryPolicy _retryPolicy;

		public HangfireAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory) : base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_retryPolicy = Policy.Handle<Exception>().Retry(5, (ex, count) =>
			{
				Logger.LogError($"Request hangfire failed [{count}]: {ex}");
			});
		}

		public bool AddOrUpdateJob(string taskId, string cron)
		{
			var url = $"{Configuration.SchedulerUrl}{(Configuration.SchedulerUrl.EndsWith("/") ? "" : "/")}Job/AddOrUpdate";
			var json = JsonConvert.SerializeObject(new HangfireJobDto
			{
				Name = taskId.ToString(),
				Cron = cron,
				Url = $"{Configuration.SchedulerCallbackHost}{(Configuration.SchedulerCallbackHost.EndsWith("/") ? "" : "/")}Task/Fire",
				Data = taskId.ToString()
			});
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				return _retryPolicy.Execute(() =>
				{
					var result = Util.Client.PostAsync(url, content).Result;
					result.EnsureSuccessStatusCode();
					return true;
				});
			}
			catch
			{
				return false;
			}
		}

		public void RemoveJob(string taskId)
		{
			var url = $"{Configuration.SchedulerUrl}{(Configuration.SchedulerUrl.EndsWith("/") ? "" : "/")}Job/Remove";
			var postData = $"jobId={taskId}";
			_retryPolicy.Execute(() =>
			{
				var content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
				var result = Util.Client.PostAsync(url, content).Result;
				result.EnsureSuccessStatusCode();
			});
		}
	}
}
