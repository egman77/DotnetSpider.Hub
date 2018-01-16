using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.TaskHistory
{
	public interface ITaskHistoryAppService
	{
		PaginationQueryDto Query(PaginationQueryTaskHistoryInput input);
		void Add(AddTaskHistoryInput taskHistory);
	}
}
