using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using NLog.Web;

namespace DotnetSpider.Enterprise
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

			try
			{
				string hostUrl = "http://*:5000";
				if (File.Exists(Path.Combine(AppContext.BaseDirectory, "domain")))
				{
					hostUrl = File.ReadAllLines("domain")[0];
				}

				var host = new WebHostBuilder()
					.UseKestrel()
					.UseContentRoot(Directory.GetCurrentDirectory())
					.UseIISIntegration()
					.UseStartup<Startup>()
					.UseApplicationInsights()
					.UseUrls(hostUrl).UseNLog()
					.Build();

				host.Run();
			}
			catch (Exception e)
			{
				logger.Error(e, "Stopped program because of exception");
				throw;
			}
		}
	}
}
