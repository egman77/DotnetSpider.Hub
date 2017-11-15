using NLog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Agent
{
	public class Messsage
	{
		public const string RunName = "RUN";
		public const string CancelName = "CANCEL";
		public const string ExitName = "EXIT";

		public virtual string NodeId { get; set; }
		public virtual string Name { get; set; }
		public virtual string Arguments { get; set; }
		public virtual string ApplicationName { get; set; }
		public virtual string Version { get; set; }
		public virtual long TaskId { get; set; }
		public virtual DateTime CreationTime { get; set; }
	}
}