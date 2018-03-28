using DotnetSpider.Enterprise.Application.TaskLog.Dto;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.TaskLog
{
	public interface ITaskLogAppService
	{
		void Add(AddTaskLogInput input);

		PaginationQueryDto Find(PaginationQueryInput input);
	}
}
