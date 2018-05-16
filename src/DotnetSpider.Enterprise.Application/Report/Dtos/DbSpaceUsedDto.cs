namespace DotnetSpider.Enterprise.Application.Report.Dtos
{
	public class DbSpaceUsedDto
	{
		public string Name { get; set; }
		public string Rows { get; set; }
		public string Reserved { get; set; }
		public string Data { get; set; }
		public string Index_Size { get; set; }
		public string Unused { get; set; }
	}
}
