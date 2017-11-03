using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using DotnetSpider.Enterprise.Web;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotnetSpider.Enterprise.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Application;
using DotnetSpider.Enterprise.Web.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using DotnetSpider.Enterprise.Domain;

namespace DotnetSpider.Enterprise
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

			//if (env.IsDevelopment())
			//{
			// For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
			//	builder.AddUserSecrets<Startup>();
			//}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

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
			services.AddMvc();

			DependencyInjectionConfig.Inject(services);
		
			//var redisHost = Configuration.GetSection(ConfigurationConsts.DefaultSetting).GetValue<string>(ConfigurationConsts.RedisHost);
			//var redisPort = Configuration.GetSection(ConfigurationConsts.DefaultSetting).GetValue<string>(ConfigurationConsts.RedisPort);
			//var redisPassword = Configuration.GetSection(ConfigurationConsts.DefaultSetting).GetValue<string>(ConfigurationConsts.RedisPassword);

			//services.AddSingleton<IDistributedCache>(
			//	serviceProvider =>
			//		new RedisCache(new RedisCacheOptions
			//		{
			//			Configuration = $"{redisHost}:{redisPort},password={redisPassword}",
			//			InstanceName = "DotnetSpiderEnterprise-Web-Session:"
			//		})
			//	);

			//Session服务
			services.AddSession();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			DI.IocManager = app.ApplicationServices;

			var config = app.ApplicationServices.GetRequiredService<ICommonConfiguration>();
			config.AppConfiguration = Configuration;

			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

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

			AuthConfigure.Configure(app, Configuration);

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

			AutoMapperConfiguration.CreateMap();

			if (env.IsDevelopment())
			{
				SeedData.Initialize(app.ApplicationServices);
			}
		}
	}
}
