using DotnetSpider.Hub.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Hub.Application.TaskHistory.Dtos
{
	public class PaginationQueryTaskHistoryInput : PaginationQueryInput
	{
		public long TaskId { get; set; }
	}
}
