using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DotnetSpider.Hub
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var configurationFile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ?
				"appsettings.Development.json" : "appsettings.json";

			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile(configurationFile, optional: true)
				.Build();

			var host = WebHost.CreateDefaultBuilder(args).UseConfiguration(config)
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseStartup<Startup>().Build();
			host.Run();
		}
	}
}
