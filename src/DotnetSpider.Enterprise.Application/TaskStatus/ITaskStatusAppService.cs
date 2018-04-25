using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Core;

namespace DotnetSpider.Enterprise.Application.TaskStatus
{
	public interface ITaskStatusAppService
	{
		void AddOrUpdate(AddOrUpdateTaskStatusInput input);
		PaginationQueryDto Find(PaginationQueryInput input);
	}
}
