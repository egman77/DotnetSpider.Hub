namespace DotnetSpider.Enterprise.Domain
{
	public class PagingQueryOutputDto
	{
		public virtual long Total { get; set; }
		public virtual int Page { get; set; }
		public virtual int Size { get; set; }
		public virtual dynamic Result { get; set; }
	}
}
