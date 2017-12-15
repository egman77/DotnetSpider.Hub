using System.Linq;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

		protected JsonResult Success()
		{
			return new JsonResult(new { Success = true });
		}

		protected JsonResult DataResult(dynamic data, string message = null)
		{
			return new JsonResult(new { Success = true, Result = data, Message = message });
		}
	}
}
