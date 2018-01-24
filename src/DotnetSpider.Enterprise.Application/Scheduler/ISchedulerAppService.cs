namespace DotnetSpider.Enterprise.Application.Scheduler
{
	public interface ISchedulerAppService
	{
		void Create(string taskId, string cron);
		void Update(string taskId, string cron);
		void Delete(string taskId);
	}
}
