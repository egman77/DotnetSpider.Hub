using SYSTEM = System.Threading;

namespace DotnetSpider.Enterprise.Application.AuthMessage
{
	public interface ISmsSender
	{
		SYSTEM.Tasks.Task<bool> SendSmsAsync(string number, string message);
	}
}
