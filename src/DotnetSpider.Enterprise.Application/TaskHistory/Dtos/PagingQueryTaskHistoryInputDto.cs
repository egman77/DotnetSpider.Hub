using DotnetSpider.Enterprise.Application.Task.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskHistory.Dtos
{
	public class PagingQueryTaskHistoryInputDto : PagingQueryTaskInputDto
	{
		[Required]
		public long TaskId { get; set; }
	}
}
