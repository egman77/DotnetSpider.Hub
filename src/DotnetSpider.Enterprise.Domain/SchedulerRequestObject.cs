using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain
{
	public class SchedulerRequestObject
	{
		public long Id { get; set; }

		public string Cron { get; set; }

		public string Url { get; set; }

		public string Data { get; set; }
	}

	public class SchedulerResponseObject
	{
		public long Id { get; set; }

		public string Data { get; set; }
	}
}
