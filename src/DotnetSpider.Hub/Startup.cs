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
				.WriteTo.Console().WriteTo.File("DotnetSpider.Hub.log")
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
				options.Lockout.MaxFailedAccessAttempts = 6;
				options.Lockout.AllowedForNewUsers = true;
			});

			// Add framework services.
			services.AddMvc(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); });

			services.AddDotnetSpiderHub(config =>
			{
				config.UseConfiguration(_configuration);
				config.UseDotnetSpiderHubServices();
			});

			return services.BuildAspectCoreServiceProvider();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseHttpProfiler();

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

			app.UseSeeData();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
