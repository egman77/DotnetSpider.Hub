using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskStatus
{
	public interface ITaskStatusAppService
	{
		void AddOrUpdate(AddOrUpdateTaskStatusInputDto input);
	}
}
