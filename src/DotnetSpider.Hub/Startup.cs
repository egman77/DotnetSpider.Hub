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
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
            //如果不是开发环境
			if (!_env.IsDevelopment())
			{
                
				Action<ApplicationOptions> application = options =>
				{
					options.ApplicationName = _env.ApplicationName; //应用程序名
					options.Environment = _env.EnvironmentName; //环境名(机器名)
				};

                //添加APM,APM是什么?
				services.AddAspectCoreAPM(component =>
				{
					component.AddLineProtocolCollector(options => 
                        Configuration.GetLineProtocolSection()
                            .Bind(options)) //选项
							.AddHttpProfiler() //http配置
							.AddApplicationProfiler();//应用程序配置
				}, application);

			}

            //添加响应快取
			services.AddResponseCaching();
            //添加响应压缩
			services.AddResponseCompression();

            //添加数据持久层上下文
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DotnetSpiderHub")));

            //添加标识符关系
			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				options.Password.RequireNonAlphanumeric = false; //必须含有特殊字母
				options.Password.RequireUppercase = false;//必须含有大写
                options.Password.RequireDigit = false;//必须含有数字
				options.Password.RequiredUniqueChars =6;//必须至少唯一字符串字数
			})
			.AddEntityFrameworkStores<ApplicationDbContext>() //添加实体模型持久层
			.AddErrorDescriber<CustomIdentityErrorDescriber>()//添加自定义错误描述
			.AddDefaultUI();

            //添加http客户端
			services.AddHttpClient();

            //添加程序构建
			services.AddDotnetSpiderHub(config =>
			{
				config.UseConfiguration(Configuration);
				config.UseDotnetSpiderHubServices();//使用程序服务(自定义扩展方法)
			});

            //添加mvc
			services.AddMvc(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); })
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //构建服务提供者
			return services.BuildAspectCoreServiceProvider();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
            //开发环境下
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage(); //使用开发者异常页 
				app.UseDatabaseErrorPage(); //使用数据库错误页
			}
			else //生产环境下
			{

				app.UseExceptionHandler("/Home/Error");  //使用错误处理
				app.UseHsts(); //使用HSTS 安全协议
			}

			app.UseHttpsRedirection(); //使用https重定向
			app.UseStaticFiles(); //使用静态文件

			app.UseAuthentication(); //使用验证

			app.UseSeeData(); //使用初始化数据 (自定义的扩展方法)

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			}); //使用mvc功能
		}
	}
}
