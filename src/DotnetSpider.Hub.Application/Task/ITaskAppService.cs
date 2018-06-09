using DotnetSpider.Hub.Application.Task.Dtos;
using DotnetSpider.Hub.Core;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Application.Task
{
	public interface ITaskAppService
	{
		void Control(string taskId, ActionType type);
		PaginationQueryDto Query(PaginationQueryTaskInput input);
		void Create(CreateTaskInput item);
		void Update(UpdateTaskInput item);
		void Delete(string taskId);

		void Run(string taskId);
		void Exit(string taskId);
		void Disable(string taskId);
		void Enable(string taskId);

		void IncreaseRunning(string taskId);
		void ReduceRunning(string taskId);
		TaskDto GetTask(string taskId);
	}
}
