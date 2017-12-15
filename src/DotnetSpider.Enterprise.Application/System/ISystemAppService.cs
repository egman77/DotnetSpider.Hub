namespace DotnetSpider.Enterprise.Application.System
{
	public interface ISystemAppService
	{
		void Register();
		void Execute(string name, string arguments);
	}
}
