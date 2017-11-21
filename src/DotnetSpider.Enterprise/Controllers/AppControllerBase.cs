using DotnetSpider.Enterprise.Application.Log;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Web.Controllers
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

		protected JsonResult ErrorResult(string message = null)
		{
			return new JsonResult(new { Success = false, Message = message });
		}

		protected JsonResult SuccessResult(string message = null)
		{
			return new JsonResult(new { Success = true, Message = message });
		}

		protected JsonResult DataResult(dynamic data, string message = "")
		{
			return new JsonResult(new { Success = true, Result = data, Message = message });
		}

		protected JsonResult ActionResult(Action action)
		{
			try
			{
				action();
				return SuccessResult();
			}
			catch (DotnetSpiderException e)
			{
				return ErrorResult(e.Message);
			}
			catch (Exception e)
			{
				Logger.LogError(e.ToString());
				return ErrorResult("服务器内部错误。");
			}
		}

		protected JsonResult ActionResult<T>(Func<T> func)
		{
			try
			{
				return DataResult(func());
			}
			catch (DotnetSpiderException e)
			{
				return ErrorResult(e.Message);
			}
			catch (Exception e)
			{
				Logger.LogError(e.ToString());
				return ErrorResult(e.Message);
			}
		}

		protected JsonResult ActionResult<T0, T1>(Func<T0, T1> func, T0 arg0)
		{
			try
			{
				return DataResult(func(arg0));
			}
			catch (DotnetSpiderException e)
			{
				return ErrorResult(e.Message);
			}
			catch (Exception e)
			{
				Logger.LogError(e.ToString());
				return ErrorResult("服务器内部错误。");
			}
		}

		protected JsonResult ActionResult<T0, T1, T2>(Func<T0, T1, T2> func, T0 arg0, T1 arg1)
		{
			try
			{
				return DataResult(func(arg0, arg1));
			}
			catch (DotnetSpiderException e)
			{
				return ErrorResult(e.Message);
			}
			catch (Exception e)
			{
				Logger.LogError(e.ToString());
				return ErrorResult("服务器内部错误。");
			}
		}

		protected JsonResult ActionResult<T0, T1, T2, T3>(Func<T0, T1, T2, T3> func, T0 arg0, T1 arg1, T2 arg2)
		{
			try
			{
				return DataResult(func(arg0, arg1, arg2));
			}
			catch (DotnetSpiderException e)
			{
				return ErrorResult(e.Message);
			}
			catch (Exception e)
			{
				Logger.LogError(e.ToString());
				return ErrorResult("服务器内部错误。");
			}
		}

		protected JsonResult ActionResult<T0, T1, T2, T3, T4>(Func<T0, T1, T2, T3, T4> func, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			try
			{
				return DataResult(func(arg0, arg1, arg2, arg3));
			}
			catch (DotnetSpiderException e)
			{
				return ErrorResult(e.Message);
			}
			catch (Exception e)
			{
				Logger.LogError(e.ToString());
				return ErrorResult("服务器内部错误。");
			}
		}

		protected JsonResult IdentityResultResult(IdentityResult result)
		{
			if (result.Succeeded)
			{
				return SuccessResult();
			}
			else
			{
				var message = GetIdentityResultError(result);
				return ErrorResult(message);
			}
		}
	}
}
