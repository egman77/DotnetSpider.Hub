using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Hangfire.Dtos
{
	public class HangfireJobDto
	{
		public virtual string Name { get; set; }

		public virtual string Cron { get; set; }

		public virtual string Url { get; set; }

		public virtual string Data { get; set; }
	}
}
