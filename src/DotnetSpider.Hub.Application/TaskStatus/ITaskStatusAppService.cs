using DotnetSpider.Hub.Application.TaskStatus.Dtos;
using DotnetSpider.Hub.Core;

namespace DotnetSpider.Hub.Application.TaskStatus
{
	public interface ITaskStatusAppService
	{
		void AddOrUpdate(AddOrUpdateTaskStatusInput input);
		PaginationQueryDto Find(PaginationQueryInput input);
	}
}
