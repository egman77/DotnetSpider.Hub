using System.Linq;
using System.Threading.Tasks;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise.Controllers
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

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null)
		{
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
					throw new DotnetSpiderException("用户名或密码不正确。");
				}

				if (!user.IsActive)
				{
					throw new DotnetSpiderException("帐户被禁用。");
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
							return RedirectToAction(nameof(SendSecurityCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
						}
						if (result.IsLockedOut)
						{
							Logger.LogWarning(2, "User account locked out.");
							throw new DotnetSpiderException("帐户被锁定。");
						}
						else
						{
							throw new DotnetSpiderException("用户名或密码不正确。");
						}
					}
				}
			}

			// If we got this far, something failed, redisplay form
			throw new DotnetSpiderException("用户名或密码不正确。");
		}

		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			Logger.LogInformation(4, "User logged out.");
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> SendSecurityCode(string returnUrl = null, bool rememberMe = false)
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