namespace DotnetSpider.Enterprise.Domain
{
	public class PaginationQueryDto
	{
		public virtual long Total { get; set; }
		public virtual int Page { get; set; }
		public virtual int Size { get; set; }
		public virtual dynamic Result { get; set; }
	}
}
