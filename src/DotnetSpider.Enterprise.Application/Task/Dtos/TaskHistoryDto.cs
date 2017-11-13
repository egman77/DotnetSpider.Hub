using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class TaskHistoryDto
	{
		public long TaskId { get; set; }

		public string Identity { get; set; }

		public string NodeIds { get; set; }
	}
}
