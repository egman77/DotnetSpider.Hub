namespace DotnetSpider.Hub.Application.System
{
	public interface ISystemAppService
	{
		void Register();
		void Execute(string name, string arguments);
		void UpgradeScheduler();
	}
}
