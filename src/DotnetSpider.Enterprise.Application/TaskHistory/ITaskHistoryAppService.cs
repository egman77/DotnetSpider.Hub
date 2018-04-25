using DotnetSpider.Enterprise.Application.TaskHistory.Dtos;
using DotnetSpider.Enterprise.Core;

namespace DotnetSpider.Enterprise.Application.TaskHistory
{
	public interface ITaskHistoryAppService
	{
		PaginationQueryDto Find(PaginationQueryInput input);
		void Add(AddTaskHistoryInput taskHistory);
	}
}
