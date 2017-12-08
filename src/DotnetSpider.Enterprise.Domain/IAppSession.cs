using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DotnetSpider.Enterprise.Domain
{
	public interface IAppSession
	{
		long? UserId { get; }
		long GetUserId();
		ClaimsPrincipal ClaimsPrincipal { get; }
		HttpRequest GetRequest();
	}
}
