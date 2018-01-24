using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application.Log.Dto
{
	public class PaginationQueryLogDto
	{
		public List<string> Columns { get; set; }
		public List<List<string>> Values { get; set; }

		public long Total { get; set; }
		public int Page { get; set; }
		public int Size { get; set; }
	}
}
