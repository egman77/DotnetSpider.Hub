namespace DotnetSpider.Enterprise.Application.AuthMessage
{
	public interface IEmailSender
    {
        System.Threading.Tasks.Task SendEmailAsync(string email, string subject, string message);
    }
}
