using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskHistory
{
	public interface ITaskHistoryAppService
	{
		PagingQueryOutputDto Query(PagingQueryTaskHistoryInputDto input);
		void Add(AddTaskHistoryInputDto taskHistory);
	}
}
