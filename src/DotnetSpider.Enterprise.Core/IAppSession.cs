using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DotnetSpider.Enterprise.Core
{
	public interface IAppSession
	{
		long? UserId { get; }
		ClaimsPrincipal ClaimsPrincipal { get; }
		HttpRequest Request { get; }
	}
}
