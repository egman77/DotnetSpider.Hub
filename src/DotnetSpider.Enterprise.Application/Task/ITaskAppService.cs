using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application.Task
{
	public interface ITaskAppService
	{
		PagingQueryOutputDto Query(PagingQueryTaskInputDto input);
		void Add(AddTaskInputDto item);
		void Modify(ModifyTaskInputDto item);

		void Run(long taskId);
		void Exit(long taskId);
		void Remove(long taskId);

		void Disable(long taskId);
		void Enable(long taskId);

		void IncreaseRunning(TaskIdInputDto input);
		void ReduceRunning(TaskIdInputDto input);

		PagingQueryOutputDto QueryRunning(PagingQueryInputDto input);
		AddTaskInputDto Get(long taskId);
		void UpgradeScheduler();
	}
}
