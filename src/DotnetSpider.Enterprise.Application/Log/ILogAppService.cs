using DotnetSpider.Enterprise.Application.Log.Dto;
using MongoDB.Bson;

namespace DotnetSpider.Enterprise.Application.Log
{
	public interface ILogAppService
	{
		void Log(Domain.Entities.Logs.Exception ex);
		void Sumit(LogInputDto input);
	}
}
