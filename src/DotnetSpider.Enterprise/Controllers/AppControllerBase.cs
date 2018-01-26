using System.Linq;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
{
	[Authorize]
	public abstract class AppControllerBase : Controller
	{
		protected readonly IAppSession Session;
		protected readonly ILogger Logger;
		protected readonly ICommonConfiguration Configuration;

		public AppControllerBase(IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
		{
			Session = appSession;
			Logger = loggerFactory.CreateLogger(GetType());
			Configuration = commonConfiguration;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!ModelState.IsValid)
			{
				throw new DotnetSpiderException($"Error parameters: {GetModelStateError()}.");
			}
			base.OnActionExecuting(context);
		}

		protected string GetModelStateError()
		{
			foreach (var item in ModelState.Values)
			{
				if (item.Errors.Count > 0)
				{
					return item.Errors[0].ErrorMessage;
				}
			}
			return "";
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
