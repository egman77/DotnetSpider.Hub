using SYSTEM = System.Threading;

namespace DotnetSpider.Enterprise.Application.AuthMessage
{
	public interface IEmailSender
	{
		SYSTEM.Tasks.Task SendEmailAsync(string email, string subject, string message);
	}
}
