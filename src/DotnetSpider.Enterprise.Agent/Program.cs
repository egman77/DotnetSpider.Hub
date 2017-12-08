using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Config;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
	public class Program
	{
		static void Main(string[] args)
		{
			var agent = new AgentClient();
			agent.Run();
		}
	}
}
