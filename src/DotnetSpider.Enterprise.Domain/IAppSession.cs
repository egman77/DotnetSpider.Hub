using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetSpider.Enterprise.Domain
{
	public interface IAppSession
	{
		long? UserId { get; }
		ClaimsPrincipal ClaimsPrincipal { get; }
		HttpRequest Request { get; }
	}
}
