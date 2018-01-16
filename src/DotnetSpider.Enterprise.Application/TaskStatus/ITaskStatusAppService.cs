using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.TaskStatus
{
	public interface ITaskStatusAppService
	{
		void AddOrUpdate(AddOrUpdateTaskStatusInput input);
		PaginationQueryDto Query(PaginationQueryTaskStatusInput input);
	}
}
