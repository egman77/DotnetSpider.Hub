using CommandLine;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading;

namespace DotnetSpider.Hub.Agent
{
	public class Options
	{
		[Option('c', "config", Required = false, HelpText = "Config file")]
		public string Config { get; set; }

		[Option('d', "daemon", Required = false, HelpText = "Run as daemon")]
		public bool Daemon { get; set; }
	}

	public class Program
	{
		static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.WriteTo.RollingFile(Path.Combine(Directory.GetCurrentDirectory(), "{Date}.log"))
				.WriteTo.Console()
				.CreateLogger();


			Parser parser = new Parser(config =>
			{
				config.CaseSensitive = false;
				config.EnableDashDash = false;
				config.CaseInsensitiveEnumValues = false;
			});

			var result = parser.ParseArguments<Options>(args);
			if (result.Tag == ParserResultType.Parsed)
			{
				ThreadPool.SetMaxThreads(256, 256);
				var options = result as Parsed<Options>;
				var agent = new AgentClient(options.Value);
				agent.Run();
			}
			else
			{
				Log.Logger.Error("Arguments uncorrect.");
			}
		}
	}
}
