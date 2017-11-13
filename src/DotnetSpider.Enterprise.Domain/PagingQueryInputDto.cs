using System;
using System.Collections.Generic;
using System.Text;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetSpider.Enterprise.Domain
{
	public class PagingQueryInputDto
	{
		private readonly ICommonConfiguration _configuration;

		public int Page { get; set; }
		public int Size { get; set; }
		public string Sort { get; set; }

		public PagingQueryInputDto(ICommonConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void Validate()
		{
			if (Page <= 0)
			{
				Page = 1;
			}

			if (Size > _configuration.PageMaxSize)
			{
				Size = _configuration.PageMaxSize;
			}

			if (Size <= 0)
			{
				Size = _configuration.PageSize;
			}
		}

		public bool IsSortByDesc()
		{
			return string.IsNullOrEmpty(Sort) ? false : Sort.ToLower().Contains("desc");
		}

		public string SortKey
		{
			get
			{
				return string.IsNullOrEmpty(Sort) ? null : Sort.ToLower().Replace("_desc", "");
			}
		}
	}
}
