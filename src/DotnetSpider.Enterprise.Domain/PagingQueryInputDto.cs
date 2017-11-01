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
		public int Page { get; set; }
		public int Size { get; set; }
		public string Sort { get; set; }

		private static readonly ICommonConfiguration Configuration;

		static PagingQueryInputDto()
		{
			Configuration = DI.IocManager.GetRequiredService<ICommonConfiguration>();
		}

		public void Init()
		{
			if (Page <= 0)
			{
				Page = 1;
			}

			if (Size > Configuration.PageMaxSize)
			{
				Size = Configuration.PageMaxSize;
			}

			if (Size <= 0)
			{
				Size = Configuration.PageSize;
			}
		}

		public bool SortByDesc()
		{
			return "desc" == Sort?.ToLower().Trim();
		}
	}
}
