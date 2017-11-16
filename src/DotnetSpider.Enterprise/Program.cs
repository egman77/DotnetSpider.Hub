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

namespace DotnetSpider.Enterprise
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string hostUrl = "http://*:5000";
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				if (File.Exists(Path.Combine(AppContext.BaseDirectory, "host.config")))
				{
					hostUrl = File.ReadAllLines("host.config")[0];
				}
			}

			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.UseApplicationInsights()
				.UseUrls(hostUrl)
				.Build();

			host.Run();
		}
	}
}
