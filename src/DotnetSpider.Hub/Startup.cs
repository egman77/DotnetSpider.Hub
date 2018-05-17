using System;
using AspectCore.APM.ApplicationProfiler;
using AspectCore.APM.AspNetCore;
using AspectCore.APM.Core;
using AspectCore.APM.HttpProfiler;
using AspectCore.APM.LineProtocolCollector;
using AspectCore.Extensions.DependencyInjection;
using DotnetSpider.Hub.Application;
using DotnetSpider.Hub.Configuration;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Core.Entities;
using DotnetSpider.Hub.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DotnetSpider.Hub
{
	public class Startup
	{
		private readonly IConfiguration _configuration;
		private readonly IHostingEnvironment _env;

		public Startup(IHostingEnvironment env, IConfiguration configuration)
		{
			_env = env;
			_configuration = configuration;

			//if (env.IsDevelopment())
			//{
			// For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
			//	builder.AddUserSecrets<Startup>();
			//}

			Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().ReadFrom.Configuration(_configuration)
				.WriteTo.Console().WriteTo.File("DotnetSpider.Enterprise.log")
				.CreateLogger();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			Action<ApplicationOptions> application = options =>
			{
				options.ApplicationName = _env.ApplicationName;
				options.Environment = _env.EnvironmentName;
			};

			services.AddAspectCoreAPM(component =>
			{
				component.AddLineProtocolCollector(options => _configuration.GetLineProtocolSection().Bind(options))
						 .AddHttpProfiler()
						 .AddApplicationProfiler();
			}, application);

			services.AddResponseCaching();
			services.AddResponseCompression();

			services.AddSingleton<IXmlRepository, XmlRepository>();
			services.AddDataProtection(configure =>
			{
				configure.ApplicationDiscriminator = DotnetSpiderConsts.Settings;
			});

			services.AddEntityFrameworkSqlServer()
				.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), b => b.UseRowNumberForPaging()));

			services.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders()
				.AddErrorDescriber<CustomIdentityErrorDescriber>();

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
				options.AccessDeniedPath = "/Account/AccessDenied";//被拒绝访问或路径无效后的重定向。
				options.ReturnUrlParameter = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.ReturnUrlParameter;//登陆或退出后执行动作返回到原来的地址。
			});

			// Add framework services.
			services.AddMvc(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); });

			//Session服务
			services.AddSession();

			services.AddDotnetSpiderEnterprise(config =>
			{
				config.UseConfiguration(_configuration);
				config.UserDotnetSpiderEnterpriseServices();
			});

			return services.BuildAspectCoreServiceProvider();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseHttpProfiler();

			loggerFactory.AddConsole(_configuration.GetSection("Logging"));
			loggerFactory.AddDebug();
			loggerFactory.AddSerilog();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			AuthConfigure.Configure(app, _configuration);

			app.UseStaticFiles();

			app.UseAuthentication();

			//Session
			app.UseSession();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			if (env.IsDevelopment())
			{
				SeedData.Initialize(app.ApplicationServices);
			}
			if (!env.IsDevelopment())
			{
				SeedData.InitializeScheduler(app.ApplicationServices);
			}
		}
	}
}
