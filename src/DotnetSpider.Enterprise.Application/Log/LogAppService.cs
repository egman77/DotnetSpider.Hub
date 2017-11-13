using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DotnetSpider.Enterprise.Application.Log
{
	public class LogAppService : AppServiceBase, ILogAppService
	{
		public LogAppService(ApplicationDbContext dbcontext)
			: base(dbcontext)
		{

		}

		public async void Sumit(LogInputDto input)
		{
			var client = new MongoClient(Configuration.LogMongoConnectionString);
			var database = client.GetDatabase("dotnetspider");
			var collection = database.GetCollection<BsonDocument>(input.Identity);

			await collection.InsertOneAsync(BsonDocument.Parse(input.LogInfo.ToString()));
		}
	}
}
