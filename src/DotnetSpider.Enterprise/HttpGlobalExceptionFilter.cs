using DotnetSpider.Enterprise.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DotnetSpider.Enterprise
{
	public class HttpGlobalExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<HttpGlobalExceptionFilter> _logger;

		public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			_logger.LogError(context.Exception.ToString());
			context.HttpContext.Response.StatusCode = 206;
			string info;
			if (context.Exception is DotnetSpiderException)
			{
				info = $"{{\"success\" : false, \"message\" : \"{context.Exception.Message}\"}}";
			}
			else
			{
				info = $"{{\"success\" : false, \"message\" : \"内部错误\"}}";
			}
			var bytes = Encoding.UTF8.GetBytes(info);
			context.ExceptionHandled = true;
			context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
			context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
		}
	}
}

