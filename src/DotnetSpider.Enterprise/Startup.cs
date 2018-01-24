using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotnetSpider.Enterprise.Domain.Entities;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Application;
using AspectCore.APM.AspNetCore;
using AspectCore.APM.LineProtocolCollector;
using AspectCore.APM.HttpProfiler;
using AspectCore.APM.ApplicationProfiler;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.APM.Core;
using DotnetSpider.Enterprise.Configuration;
using NLog.Extensions.Logging;
using NLog;
using System.Text;

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
					.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
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
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var env = (IHostingEnvironment)services.First(s => s.ServiceType == typeof(IHostingEnvironment)).ImplementationInstance;
			Action<ApplicationOptions> application = options =>
			{
				options.ApplicationName = env.ApplicationName;
				options.Environment = env.EnvironmentName;
			};
			services.AddAspectCoreAPM(component =>
			{
				component.AddLineProtocolCollector(options => Configuration.GetLineProtocolSection().Bind(options))
						 .AddHttpProfiler()
						 .AddApplicationProfiler();
			}, application);

			services.AddResponseCaching();
			services.AddResponseCompression();

			services.AddSingleton<IXmlRepository, XmlRepository>();
			services.AddDataProtection(configure =>
			{
				configure.ApplicationDiscriminator = DotnetSpiderConsts.DefaultSetting;
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
			services.AddMvc(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); });

			DependencyInjectionConfig.Inject(services);

			//Session服务
			services.AddSession();

			return services.BuildAspectCoreServiceProvider();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseHttpProfiler();

			var config = app.ApplicationServices.GetRequiredService<ICommonConfiguration>();
			config.AppConfiguration = Configuration;
			config.Tokens = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting).GetValue<string>(DotnetSpiderConsts.Tokens).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(t => !string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t)).ToArray();

			var code = Configuration.GetSection(DotnetSpiderConsts.DefaultSetting).GetValue<string>(DotnetSpiderConsts.SqlEncryptCode).Trim();
			config.SqlEncryptKey = Encoding.ASCII.GetBytes(code);

			LogManager.Configuration.Variables["connectionString"] = Configuration.GetSection("ConnectionStrings").GetValue<string>(DotnetSpiderConsts.ConnectionName);

			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();
			loggerFactory.AddNLog();

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
			else
			{
				SeedData.InitializeScheduler(app.ApplicationServices);
			}
		}
	}
}
