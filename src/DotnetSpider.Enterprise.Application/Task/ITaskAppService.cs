using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Task
{
	public interface ITaskAppService
	{
		void Control(long taskId, ActionType type);
		PaginationQueryDto Find(PaginationQueryInput input);
		void Create(CreateTaskInput item);
		void Update(UpdateTaskInput item);
		void Delete(long taskId);
		CreateTaskInput Find(long taskId);

		void Run(long taskId);
		void Exit(long taskId);
		void Disable(long taskId);
		void Enable(long taskId);

		void IncreaseRunning(long taskId);
		void ReduceRunning(long taskId);
		void UpgradeScheduler();
	}
}
