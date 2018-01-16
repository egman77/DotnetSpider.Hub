using DotnetSpider.Enterprise.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class PaginationQueryTaskInput : PaginationQueryInput
	{
		public string Keyword { get; set; }
	}
}
