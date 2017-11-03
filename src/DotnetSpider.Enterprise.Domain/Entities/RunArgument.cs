using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{

	/// <summary>
	/// 任务执行参数
	/// </summary>
	public class RunArgument
	{
		public int SolutionId { get; set; }

		public long TaskId { get; set; }

		public string Entry { get; set; }

		public string SpiderName { get; set; }

		public string ExecuteArguments { get; set; }

		public string ProjectName { get; set; }

		public string Version { get; set; }

		public string Identity { get; set; }

		public int NodeCount { get; set; }

		public string FrameworkVersion { get; set; }
	}
}
