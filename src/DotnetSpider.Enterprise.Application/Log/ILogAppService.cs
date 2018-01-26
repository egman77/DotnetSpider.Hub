using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise.Application.Log
{
	public interface ILogAppService
	{
		void Add(AddLogInput input);

		PaginationQueryLogDto Find(PaginationQueryInput input);

		void Clear();
	}
}
