using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Core.Configuration;

namespace DotnetSpider.Enterprise.Web.Controllers
{
	[Authorize]
	public class AccountController : AppControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;


		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IAppSession appSession, ILoggerFactory loggerFactory, ICommonConfiguration commonConfiguration)
			: base(appSession, loggerFactory, commonConfiguration)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		//public IActionResult Index()
		//{
		//	return View();
		//}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			//await _signInManager.SignOutAsync();

			//ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				var user = _userManager.Users.FirstOrDefault(u => u.Email == model.Email);

				if (user == null)
				{
					return ErrorResult("用户名或密码不正确。");
				}

				if (!user.IsActive)
				{
					return ErrorResult("帐户被禁用。");
				}
				else
				{
					if (!user.EmailConfirmed)
					{
						return Json(new { Success = true, ReturnUrl = $"/Account/EmailNotConfirmed?Email={user.Email}" });
					}
					else
					{
						var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
						if (result.Succeeded)
						{
							return Json(new { Success = true, ReturnUrl = "/Home" });
						}

						if (result.RequiresTwoFactor)
						{
							return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
						}
						if (result.IsLockedOut)
						{
							Logger.LogWarning(2, "User account locked out.");
							return ErrorResult("帐户被锁定。");
						}
						else
						{
							return ErrorResult("用户名或密码不正确。");
						}
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return ErrorResult("用户名或密码不正确。");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
		{
			var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				return View("Error");
			}
			var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}
	}
}