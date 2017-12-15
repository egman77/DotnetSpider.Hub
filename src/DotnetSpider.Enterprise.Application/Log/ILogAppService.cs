using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;

namespace DotnetSpider.Enterprise.Application.Log
{
	public interface ILogAppService
	{
		void Sumit(LogInputDto input);

		PagingLogOutDto Query(PagingLogInputDto input);

		void Clear();
	}
}
