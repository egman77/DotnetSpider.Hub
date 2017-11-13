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
		public PagingQueryTaskInputDto(ICommonConfiguration configuration) : base(configuration)
		{
		}

		public string Keyword { get; set; }
		public long SolutionId { get; set; }
	}

	public class PagingQueryTaskVersionInputDto : PagingQueryTaskInputDto
	{
		public PagingQueryTaskVersionInputDto(ICommonConfiguration configuration) : base(configuration)
		{
		}

		[Required]
		public long TaskId { get; set; }
	}
}
