using DotnetSpider.Enterprise.Application.TaskRunning.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskRunning
{
	public interface ITaskRunningAppService
	{
		void Add(AddTaskRunningInputDto input);
		void Remove(RemoveTaskRunningInputDto input);
	}
}
