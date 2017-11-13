using DotnetSpider.Enterprise.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskStatus.Dtos
{
	public class PagingQueryTaskStatusInputDto : PagingQueryInputDto
	{
		public string Status { get; set; }
	}
}
