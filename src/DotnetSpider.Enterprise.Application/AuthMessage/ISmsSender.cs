namespace DotnetSpider.Enterprise.Application.AuthMessage
{
	public interface ISmsSender
    {
        System.Threading.Tasks.Task<bool> SendSmsAsync(string number, string message);
    }
}
