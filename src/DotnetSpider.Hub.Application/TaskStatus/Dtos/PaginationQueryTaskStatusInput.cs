using DotnetSpider.Hub.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Hub.Application.TaskStatus.Dtos
{
	public class PaginationQueryTaskStatusInput: PaginationQueryInput
	{
		public string Keyword { get; set; }
		public string Status { get; set; }
	}
}
