using DotnetSpider.Hub.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Hub.Application.Task.Dtos
{
	public class PaginationQueryTaskInput : PaginationQueryInput
	{
		public string Keyword { get; set; }
		public bool? IsRunning { get; set; }
	}
}
