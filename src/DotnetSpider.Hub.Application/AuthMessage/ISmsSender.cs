using SYSTEM = System.Threading;

namespace DotnetSpider.Hub.Application.AuthMessage
{
	public interface ISmsSender
	{
		SYSTEM.Tasks.Task<bool> SendSmsAsync(string number, string message);
	}
}
