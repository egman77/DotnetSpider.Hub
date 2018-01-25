using DotnetSpider.Enterprise.Application.Scheduler.Dtos;

namespace DotnetSpider.Enterprise.Application.Scheduler
{
	public interface ISchedulerAppService
	{
		void Create(SchedulerJobDto job);
		void Update(SchedulerJobDto job);
		void Delete(string id);
	}
}
