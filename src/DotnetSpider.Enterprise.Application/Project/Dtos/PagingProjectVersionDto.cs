using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Project.Dtos
{
	public class PagingQueryProjectVersionInputDto : PagingQueryTaskInputDto
	{
		public PagingQueryProjectVersionInputDto(ICommonConfiguration configuration) : base(configuration)
		{
		}

		public string StartDate { get; set; }
		public string EndDate { get; set; }
	}
}
