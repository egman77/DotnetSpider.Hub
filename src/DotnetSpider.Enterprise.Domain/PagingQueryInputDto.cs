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
		public virtual int Page { get; set; }
		public virtual int Size { get; set; }
		public virtual string Sort { get; set; }

		public void Validate()
		{
			if (Page <= 0)
			{
				Page = 1;
			}

			if (Size > 60)
			{
				Size = 60;
			}

			if (Size <= 0)
			{
				Size = 40;
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
