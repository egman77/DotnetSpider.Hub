using Microsoft.AspNetCore.Http;

namespace DotnetSpider.Enterprise.Core
{
	public static class HttpContextAccessorExtensions
	{
		public static string GetRequestIp(this IHttpContextAccessor httpContextAccessor)
		{
			var ip = httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"];

			if (string.IsNullOrEmpty(ip))
			{
				ip = httpContextAccessor.HttpContext.Request.Headers["server.RemoteIpAddress"];
			}

			if (string.IsNullOrEmpty(ip))
			{
				throw new DotnetSpiderException("Can not detect client ip.");
			}
			return ip;
		}
	}
}
