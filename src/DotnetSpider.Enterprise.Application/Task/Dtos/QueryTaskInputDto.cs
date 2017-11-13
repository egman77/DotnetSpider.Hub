using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class PagingQueryTaskInputDto : PagingQueryInputDto
	{
		public string Keyword { get; set; }
	}

	public class PagingQueryTaskHistoryInputDto : PagingQueryTaskInputDto
	{
		[Required]
		public long TaskId { get; set; }
	}
}
