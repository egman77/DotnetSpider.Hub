using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

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

		public PagingLogOutDto QueryLogs(PagingLogInputDto input)
		{
			var client = new MongoClient(Configuration.LogMongoConnectionString);
			var database = client.GetDatabase("dotnetspider");
			var collection = database.GetCollection<BsonDocument>(input.Identity);

			var result = new PagingLogOutDto
			{
				Page = input.Page,
				Size = input.Size
			};

			List<BsonDocument> list = null;
			if (!string.IsNullOrEmpty(input.Node))
			{
				list = collection.Find(new BsonDocument("Node", input.Node)).Skip((input.Page - 1) * input.Size).Limit(input.Size).ToList();
				result.Total = collection.Find(new BsonDocument("Node", input.Node)).Count();
			}
			else
			{
				list = collection.Find(new BsonDocument()).Skip((input.Page - 1) * input.Size).Limit(input.Size).ToList();
				result.Total = collection.Find(new BsonDocument()).Count();
			}

			if (list.Count > 0)
			{
				var head = list.First();
				foreach (var hi in head.Elements)
				{
					result.Columns.Add(hi.Name);
				}
				foreach (var item in list)
				{
					var vlist = new List<string>();
					foreach (var v in item.Values)
					{
						vlist.Add(v.AsString);
					}
					result.Values.Add(vlist);
				}
			}
			return result;
		}
	}
}
