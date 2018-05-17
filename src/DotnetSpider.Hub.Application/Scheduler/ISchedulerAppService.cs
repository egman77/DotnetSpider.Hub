using DotnetSpider.Hub.Application.Scheduler.Dtos;

namespace DotnetSpider.Hub.Application.Scheduler
{
	public interface ISchedulerAppService
	{
		void Create(SchedulerJobDto job);
		void Update(SchedulerJobDto job);
		void Delete(string id);
	}
}
