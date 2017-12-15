using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.TaskHistory
{
	public interface ITaskHistoryAppService
	{
		PagingQueryOutputDto Query(PagingQueryTaskHistoryInputDto input);
		void Add(AddTaskHistoryInputDto taskHistory);
	}
}
