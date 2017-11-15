using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Config;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
	public class Program
	{

		static void Main(string[] args)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Console.Title = $"DotnetSpider Agent v{Config.Version}";
			}
			var agent = new AgentService();
			agent.CheckUniqueness();
			using (File.Create(Config.RunningLockPath))
			{
				agent.CheckConfig();
				agent.LoadConfig();

				if (args.Contains("--demon"))
				{
					agent.Start();
				}
				else
				{
					agent.StartAysnc();
					Console.WriteLine("Enter q: to exit:");
					while (Console.ReadLine() != "q:")
					{
						Console.WriteLine("Press q: to exit.");
					}
				}
			}
			agent.Exit();
		}
	}
}
