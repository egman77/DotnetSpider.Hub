namespace DotnetSpider.Enterprise.Application.Hangfire
{
	public interface IHangfireAppService
	{
		bool AddOrUpdateJob(string taskId, string cron);
		void RemoveJob(string taskId);
	}
}
