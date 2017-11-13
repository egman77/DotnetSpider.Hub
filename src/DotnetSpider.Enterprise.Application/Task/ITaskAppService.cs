using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application.Task
{
	public interface ITaskAppService
	{
		QueryTaskOutputDto GetList(PagingQueryTaskInputDto input);
		//PagingQueryOutputDto GetVersions(QueryTaskVersionInputDto input);
		//void SetVersion(long taskId, string version);
		void AddTask(TaskDto item);
		void ModifyTask(TaskDto item);
		//PagingQueryOutputDto GetBatches(QueryBatchInputDto input);
		//PagingQueryOutputDto GetBatchLogs(QueryBatchLogsInputDto input);
		List<RunningTaskDto> GetRunningTasks();
		void RunTask(long taskId);
		List<long> IsTaskRunning(long[] tasks);
		//List<NodeStatusDto> GetNodeStatus(string identity);
		void StopTask(string identity);
		void PauseTask(string identity);
		void ResumeTask(string identity);
		void RemoveTask(long taskId);
		bool TaskRunning(string identity);
		void ProcessCountChanged(long taskId, bool isStart);
		//void GetVersions(QueryTaskVersionInputDto input);
		//void SetVersion(long taskId, string version);
	}
}
