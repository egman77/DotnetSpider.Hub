using System.Text;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotnetSpider.Hub
{
	public class HttpGlobalExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<HttpGlobalExceptionFilter> _logger;
		private readonly ICommonConfiguration _commonConfiguration;

		public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger, ICommonConfiguration commonConfiguration)
		{
			_logger = logger;
			_commonConfiguration = commonConfiguration;
		}

		public void OnException(ExceptionContext context)
		{
			context.HttpContext.Response.StatusCode = 206;

			if (_commonConfiguration.RecordGloabException)
			{
				_logger.LogError(context.Exception.ToString());
			}

			string info;

			if (context.Exception is DotnetSpiderException)
			{
				info = JsonConvert.SerializeObject(new StandardResult { Code = 101, Message = context.Exception.Message, Status = Status.Error });
			}
			else
			{
				info = JsonConvert.SerializeObject(new StandardResult { Code = 102, Message = "Internal error.", Status = Status.Error });
			}

			var bytes = Encoding.UTF8.GetBytes(info);
			context.ExceptionHandled = true;
			context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
			context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
		}
	}
}

