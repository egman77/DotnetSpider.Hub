using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DotnetSpider.Enterprise
{
	public class Program
	{
		private static string hostUrl = "http://*:5000";

		public static void Main(string[] args)
		{
			if (File.Exists(Path.Combine(AppContext.BaseDirectory, "domain")))
			{
				hostUrl = File.ReadAllLines("domain")[0];
			}

			var host = BuildWebHost(args);

			host.Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
			.UseContentRoot(Directory.GetCurrentDirectory())
			.UseStartup<Startup>()
			.UseUrls(hostUrl)
			.Build();
	}
}
