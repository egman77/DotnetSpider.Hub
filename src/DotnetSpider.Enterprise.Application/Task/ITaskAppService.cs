using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Task
{
	public interface ITaskAppService
	{
		PaginationQueryDto Query(PaginationQueryTaskInput input);
		void Add(AddTaskInput item);
		void Modify(ModifyTaskInput item);

		void Run(long taskId);
		void Exit(long taskId);
		void Remove(long taskId);

		void Disable(long taskId);
		void Enable(long taskId);

		void IncreaseRunning(TaskIdInput input);
		void ReduceRunning(TaskIdInput input);

		PaginationQueryDto QueryRunning(PaginationQueryInput input);
		AddTaskInput Get(long taskId);
		void UpgradeScheduler();
	}
}
