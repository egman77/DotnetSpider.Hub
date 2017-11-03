using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
	public class NodeDetailDto
    {
		public string Ip { get; set; }
		public bool IsOnline { get; set; }
		public bool IsEnabled { get; set; }
		public string Os { get; set; }
		public string Version { get; set; }

		public List<PerformanceData> PerformanceData { get; set; }
	}

	public class PerformanceData
	{
		public string Time { get; set; }
		public int Cpu { get; set; }
		public int Memory { get; set; }
		public int RunningTasks { get; set; }
	}	
}
