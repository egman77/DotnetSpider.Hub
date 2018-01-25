using DotnetSpider.Enterprise.Application.Scheduler.Dtos;
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

namespace DotnetSpider.Enterprise.Application.Scheduler
{
	public class SchedulerAppService : AppServiceBase, ISchedulerAppService
	{
		private readonly RetryPolicy _retryPolicy;

		public SchedulerAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory) : base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			_retryPolicy = Policy.Handle<Exception>().Retry(5, (ex, count) =>
			{
				Logger.LogError($"Request scheduler.net failed [{count}]: {ex}");
			});
		}

		public void Create(SchedulerJobDto job)
		{
			var content = new StringContent(JsonConvert.SerializeObject(job), Encoding.UTF8, "application/json");
			try
			{
				_retryPolicy.Execute(() =>
				{
					var result = Util.Client.PostAsync(Configuration.SchedulerUrl, content).Result;
					result.EnsureSuccessStatusCode();
				});
			}
			catch
			{
				throw new DotnetSpiderException("Create scheduler failed.");
			}
		}

		public void Delete(string taskId)
		{
			var url = $"{Configuration.SchedulerUrl}{(Configuration.SchedulerUrl.EndsWith("/") ? "" : "/")}{taskId}";
			_retryPolicy.Execute(() =>
			{
				var result = Util.Client.DeleteAsync(url).Result;
				result.EnsureSuccessStatusCode();
			});
		}

		public void Update(SchedulerJobDto job)
		{
			var json = JsonConvert.SerializeObject(job);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				_retryPolicy.Execute(() =>
				{
					var result = Util.Client.PutAsync(Configuration.SchedulerUrl, content).Result;
					result.EnsureSuccessStatusCode();
				});
			}
			catch
			{
				throw new DotnetSpiderException($"Update scheduler failed.");
			}
		}
	}
}
