namespace DotnetSpider.Enterprise.Application.Log
{
	public interface IApplicationLogAppService
	{
		void Log(Domain.Entities.Logs.Exception ex);
	}
}
