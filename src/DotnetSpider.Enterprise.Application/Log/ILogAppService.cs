using DotnetSpider.Enterprise.Application.Log.Dto;
using MongoDB.Bson;

namespace DotnetSpider.Enterprise.Application.Log
{
	public interface ILogAppService
	{
		void Sumit(LogInputDto input);
	}
}
