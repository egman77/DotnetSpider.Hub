using DotnetSpider.Hub.Application.TaskLog.Dtos;
using DotnetSpider.Hub.Core;

namespace DotnetSpider.Hub.Application.TaskLog
{
	public interface ITaskLogAppService
	{
		void Add(AddTaskLogInput input);

		PaginationQueryDto Find(PaginationQueryInput input);
	}
}
