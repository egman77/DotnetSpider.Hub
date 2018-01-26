using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotnetSpider.Enterprise.Application
{
	public class AppSession : IAppSession
	{
		private readonly IHttpContextAccessor _accessor;

		public AppSession(IHttpContextAccessor accessor)
		{
			_accessor = accessor;
		}


		public long? UserId
		{
			get
			{
				var value = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (long.TryParse(value, out var userId))
				{
					return userId;
				}
				return null;
			}

		}

		public ClaimsPrincipal ClaimsPrincipal => _accessor.HttpContext.User;


		public HttpRequest Request => _accessor.HttpContext.Request;
	}
}
