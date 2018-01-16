using DotnetSpider.Enterprise.Application.Log.Dto;

namespace DotnetSpider.Enterprise.Application.Log
{
	public interface ILogAppService
	{
		void Add(AddLogInput input);

		PaginationQueryLogDto Query(PaginationQueryLogInput input);

		void Clear();
	}
}
