using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application.Task
{
	public interface ITaskAppService
	{
		QueryTaskOutputDto Query(PagingQueryTaskInputDto input);
		void Add(TaskDto item);
		void Modify(TaskDto item);

		void Run(long taskId);
		//List<NodeStatusDto> GetNodeStatus(string identity);
		void Exit(string identity);
		void Remove(long taskId);

		bool Fire(long taskId);
		bool Disable(long taskId);
		bool Enable(long taskId);

		void IncreaseRunning(TaskIdInputDto input);
		void ReduceRunning(TaskIdInputDto input);

		PagingQueryOutputDto Running(PagingQueryInputDto input);
		PagingQueryOutputDto QueryRunHistory(PagingQueryTaskHistoryInputDto input);
		TaskDto GetTask(long taskId);
	}
}
