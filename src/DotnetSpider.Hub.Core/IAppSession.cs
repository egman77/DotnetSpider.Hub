using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DotnetSpider.Hub.Core
{
	public interface IAppSession
	{
		long? UserId { get; }
		ClaimsPrincipal ClaimsPrincipal { get; }
		HttpRequest Request { get; }
	}
}
