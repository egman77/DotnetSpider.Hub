using System.Linq;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DotnetSpider.Hub.Controllers
{
	[Authorize]
	public abstract class BaseController : Controller
	{
		protected readonly IAppSession Session;
		protected readonly ILogger Logger;
		protected readonly ICommonConfiguration Configuration;

		public BaseController(IAppSession appSession, ICommonConfiguration commonConfiguration)
		{
			Session = appSession;
			Logger = Log.ForContext(GetType());
			Configuration = commonConfiguration;
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
