using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application.Task
{
	public interface ITaskAppService
	{
		QueryTaskOutputDto GetList(PagingQueryTaskInputDto input);
		void AddTask(TaskDto item);
		void ModifyTask(TaskDto item);

		void RunTask(long taskId);
		//List<NodeStatusDto> GetNodeStatus(string identity);
		void StopTask(string identity);
		void RemoveTask(long taskId);
		
		void ProcessCountChanged(long taskId, bool isStart);

		bool Fire(long taskId);
		bool Disable(long taskId);
		bool Enable(long taskId);

		void IncreaseRunning(TaskIdInputDto input);
		void ReduceRunning(TaskIdInputDto input);
	}
}
