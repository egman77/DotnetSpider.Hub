using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotnetSpider.Hub.EntityFrameworkCore;
using DotnetSpider.Hub.Core.Entities;
using Serilog;
using Serilog.Events;
using System.IO;
using AspectCore.APM.AspNetCore;
using AspectCore.APM.Core;
using AspectCore.APM.LineProtocolCollector;
using AspectCore.APM.HttpProfiler;
using AspectCore.APM.ApplicationProfiler;
using DotnetSpider.Hub.Core;
using DotnetSpider.Hub.Application;
using AspectCore.Extensions.DependencyInjection;

namespace DotnetSpider.Hub
{
	public class Startup
	{
		private readonly IHostingEnvironment _env;

		public Startup(IHostingEnvironment env, IConfiguration configuration)
		{
			_env = env;
			Configuration = configuration;

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
				.WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "DOTNETSPIDER-HUB-{Date}.log"))
				.WriteTo.Console()
				.CreateLogger();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			if (!_env.IsDevelopment())
			{
				Action<ApplicationOptions> application = options =>
				{
					options.ApplicationName = _env.ApplicationName;
					options.Environment = _env.EnvironmentName;
				};

				services.AddAspectCoreAPM(component =>
				{
					component.AddLineProtocolCollector(options => Configuration.GetLineProtocolSection().Bind(options))
							.AddHttpProfiler()
							.AddApplicationProfiler();
				}, application);

			}
			services.AddResponseCaching();
			services.AddResponseCompression();

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DotnetSpiderHub")));

			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireDigit = false;
				options.Password.RequiredUniqueChars = 6;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddErrorDescriber<CustomIdentityErrorDescriber>()
			.AddDefaultUI();

			services.AddHttpClient();

			services.AddDotnetSpiderHub(config =>
			{
				config.UseConfiguration(Configuration);
				config.UseDotnetSpiderHubServices();
			});
			services.AddMvc(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); })
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			return services.BuildAspectCoreServiceProvider();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
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
