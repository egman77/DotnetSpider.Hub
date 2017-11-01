using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotnetSpider.Enterprise
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string hostUrl = "http://*:5000";
			if (File.Exists("host.url"))
			{
				hostUrl = File.ReadAllLines("host.url")[0];
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
