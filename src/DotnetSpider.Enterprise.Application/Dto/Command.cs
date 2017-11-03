using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Dto
{
	public class Command
	{
		public const string Run = "RUN";
		public const string Publish = "PUBLISH";
		public const string DeleteProject = "DELETE_PROJECT";
		public const string DeleteTask = "DELETE_TASK";
		public const string Enable = "ENABLE";
		public const string Disable = "DISABLE";

		public string Id { get; set; }
		public string Target { get; set; }
		public string Name { get; set; }
		public string Data { get; set; }
	}

	public class PublishArgument
	{
		public long BuildId { get; set; }
		public string Url { get; set; }
		public string EntryProject { get; set; }
		public string Tags { get; set; }
	}

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
