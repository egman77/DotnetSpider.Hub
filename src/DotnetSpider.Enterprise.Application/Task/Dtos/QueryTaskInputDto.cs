using DotnetSpider.Enterprise.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class QueryTaskInputDto : PagingQueryInputDto
	{
		public string Keyword { get; set; }
		public long SolutionId { get; set; }
	}

	public class QueryTaskVersionInputDto : QueryTaskInputDto
	{
		[Required]
		public long TaskId { get; set; }
	}
}
