using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Core;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotnetSpider.Enterprise.Application.Log
{
	public class LogAppService : AppServiceBase, ILogAppService
	{
		public LogAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<Domain.Entities.ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
		}

		public async void Add(AddLogInput input)
		{
			if (input == null)
			{
				Logger.LogError($"{nameof(input)} should not be null.");
				return;
			}

			if (string.IsNullOrEmpty(input.Identity) || input.LogInfo == null)
			{
				return;
			}
			var client = new MongoClient(Configuration.LogMongoConnectionString);
			var database = client.GetDatabase("dotnetspider");
			var collection = database.GetCollection<BsonDocument>(input.Identity);
			await collection.InsertOneAsync(BsonDocument.Parse(input.LogInfo.ToString()));
		}

		public PaginationQueryLogDto Find(PaginationQueryInput input)
		{
			if (input == null)
			{
				throw new ArgumentNullException($"{nameof(input)} should not be null.");
			}
			var result = new PaginationQueryLogDto
			{
				Page = input.Page.Value,
				Size = input.Size.Value,
				Columns = new List<string>(),
				Values = new List<List<string>>()
			};
			var identity = input.GetFilterValue("identity")?.Trim();
			if (string.IsNullOrWhiteSpace(identity))
			{
				return result;
			}
			var client = new MongoClient(Configuration.LogMongoConnectionString);
			var database = client.GetDatabase("dotnetspider");
			var collection = database.GetCollection<BsonDocument>(identity);

			List<BsonDocument> list = null;
			var queryBson = new BsonDocument();
			var nodeId = input.GetFilterValue("nodeid")?.Trim();

			if (!string.IsNullOrWhiteSpace(nodeId))
			{
				queryBson.Add("NodeId", nodeId);
			}
			var logType = input.GetFilterValue("logtype");
			if (!string.IsNullOrWhiteSpace(logType) && "all" != logType.Trim().ToLower())
			{
				queryBson.Add("Level", logType);
			}

			list = collection.Find(queryBson).Skip((input.Page - 1) * input.Size).Limit(input.Size).Sort(Builders<BsonDocument>.Sort.Descending("_id")).ToList();
			result.Total = collection.Find(queryBson).Count();

			if (list.Count > 0)
			{
				var head = list.First();
				foreach (var hi in head.Elements)
				{
					if (hi.Name == "_id" || hi.Name.ToLower() == "identity")
					{
						continue;
					}
					result.Columns.Add(hi.Name);
				}
				foreach (var item in list)
				{
					var vlist = new List<string>();
					foreach (var v in item.Elements.Where(a => a.Name != "_id" && a.Name.ToLower() != "identity"))
					{
						vlist.Add(v.Value is BsonNull ? string.Empty : v.Value.ToString());
					}
					result.Values.Add(vlist);
				}
			}
			return result;
		}

		public void Clear()
		{
			var client = new MongoClient(Configuration.LogMongoConnectionString);
			var database = client.GetDatabase("dotnetspider");
			var collections = database.ListCollections().ToList();
			foreach (var collection in collections)
			{
				database.DropCollection(collection["name"].AsString);
			}
			Logger.LogInformation($"Clear logs success.");
		}
	}
}
