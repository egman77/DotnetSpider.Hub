using NLog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
	public class Command
	{
		public const string RunName = "RUN";
		public const string CanleName = "CANLE";

		public string AngentId { get; set; }
		public string Task { get; set; }
		public string Name { get; set; }
		public string Arguments { get; set; }
		public string Application { get; set; }
		public string Version { get; set; }
	}
}