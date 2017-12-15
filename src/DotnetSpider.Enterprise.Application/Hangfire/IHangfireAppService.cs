namespace DotnetSpider.Enterprise.Application.Hangfire
{
	public interface IHangfireAppService
	{
		bool AddOrUpdateHangfireJob(string taskId, string cron);
		void RemoveHangfireJob(string taskId);
	}
}
