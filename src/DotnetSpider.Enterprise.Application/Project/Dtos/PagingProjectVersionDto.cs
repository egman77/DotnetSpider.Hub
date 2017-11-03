using DotnetSpider.Enterprise.Application.Task.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Project.Dtos
{
	public class PagingProjectVersionDto : QueryTaskInputDto
	{
		public string StartDate { get; set; }
		public string EndDate { get; set; }
	}
}
