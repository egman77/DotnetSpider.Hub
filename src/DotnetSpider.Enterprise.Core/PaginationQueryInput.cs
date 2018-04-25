using DotnetSpider.Enterprise.Core.Entities;

namespace DotnetSpider.Enterprise.Core
{
	public class PaginationQueryInput : FilterQueryInput
	{
		public virtual int? Page { get; set; }
		public virtual int? Size { get; set; }
		public virtual string Sort { get; set; }
		public virtual bool SortByDesc { get; set; } = true;

		public void Validate()
		{
			if (Page == null || Page <= 0)
			{
				Page = 1;
			}

			if (Size == null || Size > 60)
			{
				Size = 60;
			}

			if (Size <= 0)
			{
				Size = 40;
			}
		}
	}
}
