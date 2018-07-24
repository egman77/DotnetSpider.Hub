using System.Linq;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Serilog;

namespace DotnetSpider.Hub.Controllers
{
	[Authorize(Roles = "Admin")]
	public abstract class BaseController : Controller
	{
		protected readonly ILogger Logger;
		protected readonly ICommonConfiguration Configuration;

		public BaseController(ICommonConfiguration commonConfiguration)
		{
			Logger = Log.ForContext(GetType());
			Configuration = commonConfiguration;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!ModelState.IsValid)
			{
				var kv = context.ActionArguments.FirstOrDefault();
				throw new DotnetSpiderHubException($"ModelState is invalid: {context.HttpContext.Request.Path}, values: {JsonConvert.SerializeObject(kv.Value)}.");
			}
			base.OnActionExecuting(context);
		}

		protected IActionResult Success()
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Success });
		}

		protected IActionResult Success(dynamic data, string message = null)
		{
			return new JsonResult(new StandardResult { Code = 100, Status = Status.Success, Data = data, Message = message });
		}

		protected void CheckAuth()
		{
			CheckAuth(Configuration.RequireToken);
		}

		protected void CheckAuth(bool require)
		{
			if (!require)
			{
				return;
			}
			if (Request.Headers.ContainsKey("HubToken"))
			{
				var token = Request.Headers["HubToken"].ToString();
				if (Configuration.Tokens.Contains(token))
				{
					return;
				}
			}

			throw new DotnetSpiderHubException("Access Denied.");
		}
	}
}
