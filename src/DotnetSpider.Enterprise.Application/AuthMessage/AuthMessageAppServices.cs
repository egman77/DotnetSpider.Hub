using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Application.AuthMessage
{
	// This class is used by the application to send Email and SMS
	// when you turn on two-factor authentication in ASP.NET Identity.
	// For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
	public class AuthMessageAppServices : IEmailSender, ISmsSender
	{
		private static HttpClient _httpClient = new HttpClient();
		private readonly ICommonConfiguration _configuration;
		private readonly IHostingEnvironment _env;

		public AuthMessageAppServices(IHostingEnvironment env)
		{
			_configuration = DI.IocManager.GetRequiredService<ICommonConfiguration>();
			_env = env;
		}

		public System.Threading.Tasks.Task SendEmailAsync(string email, string subject, string message)
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress(_configuration.EmailSenderFromDisplayName, _configuration.EmailSenderFromAddress));
			emailMessage.To.Add(new MailboxAddress(email, email));
			emailMessage.Subject = subject;
			
			emailMessage.Body = new TextPart("html") {  Text = message };

			using (var client = new MailKit.Net.Smtp.SmtpClient())
			{
				client.Connect(_configuration.EmailSenderHost, _configuration.EmailSenderPort, _configuration.EmailSenderEnableSsl);
				client.Authenticate(_configuration.EmailSenderFromAddress, _configuration.EmailSenderPassword);

				client.Send(emailMessage);
				client.Disconnect(true);

			}
			return System.Threading.Tasks.Task.FromResult(true);
		}

		public async System.Threading.Tasks.Task<bool> SendSmsAsync(string number, string message)
		{
			if (_env.IsDevelopment())
			{
				return true;
			}
			else
			{
				var postData = JsonConvert.SerializeObject(new
				{
					Phone = number,
					Code = message
				});
				var response = await _httpClient.PostAsync(_configuration.SmsApi, new StringContent(postData, Encoding.UTF8, "application/json"));

				var result = await response.Content.ReadAsStringAsync();
				return result == "true";
			}
		}
	}
}
