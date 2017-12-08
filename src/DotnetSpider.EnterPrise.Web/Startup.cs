using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotnetSpider.EnterPrise.Web.Data;
using DotnetSpider.EnterPrise.Web.Models;
using DotnetSpider.EnterPrise.Web.Services;
using Microsoft.AspNetCore.DataProtection.Repositories;
using DotnetSpider.Enterprise.Web;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.EnterPrise.Web
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			IConfigurationBuilder builder;
			if (env.IsDevelopment())
			{
				builder = new ConfigurationBuilder()
					.SetBasePath(env.ContentRootPath)
					.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
					.AddEnvironmentVariables();
			}
			else
			{
				builder = new ConfigurationBuilder()
					.SetBasePath(env.ContentRootPath)
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddEnvironmentVariables();
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddResponseCaching();
			services.AddResponseCompression();
			services.AddSingleton<IXmlRepository, XmlRepository>();

			services.AddDataProtection(configure =>
			{
				configure.ApplicationDiscriminator = ConfigurationConsts.DefaultSetting;
			});

			services.AddEntityFrameworkSqlServer()
				.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.UseRowNumberForPaging()));


			services.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
			{
				options.User.RequireUniqueEmail = true;
				// Password settings
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 6;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 6;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 10;
				options.Lockout.AllowedForNewUsers = true;

				// User settings
				options.User.RequireUniqueEmail = true;
			});

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Account/LogIn";
				options.LogoutPath = "/Account/LogOff";
				options.ExpireTimeSpan = TimeSpan.FromDays(150);//Cookie 保持有效的时间150天。
																//cookie扩展设置（通常不用）
				//options.Cookie.Domain = "DotnetSpider.Enterprise";//用于保持身份的 Cookie 名称。 默认值为“.AspNet.Cookies”。 
			});


			// Add application services.
			services.AddTransient<IEmailSender, EmailSender>();

			services.AddMvc();

			services.AddSingleton<ICommonConfiguration, CommonConfiguration>();

			services.AddSession();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			var pa1paConfiguration = app.ApplicationServices.GetRequiredService<ICommonConfiguration>();
			pa1paConfiguration.AppConfiguration = Configuration;

			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseSession();

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

		
		}
	}
}
