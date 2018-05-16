using System.IO;
using System.Linq;
using System.Text;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace DotnetSpider.Enterprise.Controllers
{
	[Authorize]
	public abstract class AppControllerBase : Controller
	{
		protected readonly IAppSession Session;
		protected readonly ILogger Logger;
		protected readonly ICommonConfiguration Configuration;

		public AppControllerBase(IAppSession appSession, ICommonConfiguration commonConfiguration)
		{
			Session = appSession;
			Logger = Log.ForContext(GetType());
			Configuration = commonConfiguration;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!ModelState.IsValid)
			{
				StringBuilder builder = new StringBuilder();
				builder.Append($"Error request Path {context.HttpContext.Request.Path}, Query: {context.HttpContext.Request.QueryString}");
				if (context.HttpContext.Request.Method.ToLower() == "post")
				{
					var memory = new MemoryStream();
					context.HttpContext.Request.Body.CopyTo(memory);
					var body = Encoding.UTF8.GetString(memory.ToArray());
					builder.Append($" , Body: {body}");
				}
				builder.Append(".");
				throw new DotnetSpiderException(builder.ToString());
			}
			base.OnActionExecuting(context);
		}

		protected string GetIdentityResultError(IdentityResult result)
		{
			var error = result.Errors.FirstOrDefault();
			return error == null ? "" : error.Description;
		}

		protected IActionResult Success()
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Success });
		}

		protected IActionResult Success(object data, string message = null)
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Success, Data = data, Message = message });
		}

		protected IActionResult Failed(string message = null)
		{
			return new JsonResult(new StandardResult { Code = 103, Status = Status.Failed, Message = message });
		}

		protected void CheckAuth()
		{
			if (!Configuration.AuthorizeApi)
			{
				return;
			}
			if (Request.Headers.ContainsKey("DotnetSpiderToken"))
			{
				var token = Session.Request.Headers["DotnetSpiderToken"].ToString();
				if (Configuration.Tokens.Contains(token))
				{
					return;
				}
			}

			throw new DotnetSpiderException("Access Denied.");
		}
	}
}
