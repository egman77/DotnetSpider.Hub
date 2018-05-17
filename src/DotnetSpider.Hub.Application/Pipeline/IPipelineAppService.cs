namespace DotnetSpider.Hub.Application.Pipeline
{
	public interface IPipelineAppService
	{
		int Process(string database, string table, string[] values);
		void CreateDatabaseAndTable(string database, string table, string[] columns);
	}
}
