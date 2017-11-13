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

	public class PagingLogInputDto : PagingQueryInputDto
	{
		[Required]
		public string Identity { get; set; }

		public string Node { get; set; }

		public string LogType { get; set; }
	}

	public class PagingLogOutDto
	{
		public List<string> Columns { get; set;}
		public List<List<string>> Values { get; set; }

		public long Total { get; set; }
		public int Page { get; set; }
		public int Size { get; set; }
	}
}
