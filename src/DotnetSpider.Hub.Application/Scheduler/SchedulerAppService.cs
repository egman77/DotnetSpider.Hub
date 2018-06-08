using System;
using System.Net.Http;
using System.Text;
using DotnetSpider.Hub.Application.Scheduler.Dtos;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace DotnetSpider.Hub.Application.Scheduler
{
	public class SchedulerAppService : AppServiceBase, ISchedulerAppService
	{
		private readonly RetryPolicy _retryPolicy;
		private readonly IHttpClientFactory _httpClientFactory;

		public SchedulerAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration,
			UserManager<ApplicationUser> userManager, IHttpClientFactory  httpClientFactory) : base(dbcontext, configuration, userManager)
		{
			_httpClientFactory = httpClientFactory;
			_retryPolicy = Policy.Handle<Exception>().Retry(5, (ex, count) =>
			{
				Logger.Error($"Request scheduler.net failed [{count}]: {ex}");
			});
		}

		public void Create(SchedulerJobDto job)
		{
			var content = new StringContent(JsonConvert.SerializeObject(job), Encoding.UTF8, "application/json");
			try
			{
				var result = _retryPolicy.Execute(() =>
				{
					var response = _httpClientFactory.CreateClient().PostAsync(Configuration.SchedulerUrl, content).Result;
					response.EnsureSuccessStatusCode();
					return response;
				});

				CheckResult(result);
			}
			catch (Exception e)
			{
				throw new DotnetSpiderHubException($"Create scheduler failed: {e.Message}.");
			}
		}

		public void Delete(string taskId)
		{
			var url = $"{Configuration.SchedulerUrl}{(Configuration.SchedulerUrl.EndsWith("/") ? "" : "/")}{taskId}";
			try
			{
				var result = _retryPolicy.Execute(() =>
				{
					var response = _httpClientFactory.CreateClient().DeleteAsync(url).Result;
					response.EnsureSuccessStatusCode();

					return response;
				});

				CheckResult(result);
			}
			catch (Exception e)
			{
				throw new DotnetSpiderHubException($"Create scheduler failed: {e.Message}.");
			}
		}

		public void Update(SchedulerJobDto job)
		{
			var json = JsonConvert.SerializeObject(job);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			try
			{
				var result = _retryPolicy.Execute(() =>
				{
					var response = _httpClientFactory.CreateClient().PutAsync(Configuration.SchedulerUrl, content).Result;
					response.EnsureSuccessStatusCode();
					return response;
				});

				CheckResult(result);
			}
			catch (Exception e)
			{
				throw new DotnetSpiderHubException($"Create scheduler failed: {e.Message}.");
			}
		}

		private void CheckResult(HttpResponseMessage response)
		{
			var str = response.Content.ReadAsStringAsync().Result;
			var json = JsonConvert.DeserializeObject<StandardResult>(str);
			if (json.Status != Status.Success)
			{
				throw new Exception(json.Message);
			}
		}
	}
}
