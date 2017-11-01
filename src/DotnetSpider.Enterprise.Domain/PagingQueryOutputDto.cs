namespace DotnetSpider.Enterprise.Domain
{
	public class PagingQueryOutputDto
	{
		public long Total { get; set; }
		public int Page { get; set; }
		public int Size { get; set; }
		public dynamic Result { get; set; }
	}
}
