using DotnetSpider.Hub.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Hub.Application.TaskLog.Dtos
{
	public class PaginationQueryTaskLogInput : PaginationQueryInput
	{
		[Required]
		public string Identity { get; set; }

		public string NodeId { get; set; }

		public string LogType { get; set; }
	}
}
