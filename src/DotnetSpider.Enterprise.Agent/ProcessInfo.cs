using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DotnetSpider.Enterprise.Agent
{
	public class ProcessInfo
	{
		public virtual string TaskId { get; set; }
		public virtual Process Process { get; set; }
		public virtual string WorkingDirectory { get; set; }
	}
}
