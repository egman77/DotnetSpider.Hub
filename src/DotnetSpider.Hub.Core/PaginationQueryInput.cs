using DotnetSpider.Hub.Core.Entities;

namespace DotnetSpider.Hub.Core
{
	public class PaginationQueryInput
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

			if (Size == null || Size > 30)
			{
				Size = 30;
			}

			if (Size <= 0)
			{
				Size = 30;
			}
		}
	}
}
