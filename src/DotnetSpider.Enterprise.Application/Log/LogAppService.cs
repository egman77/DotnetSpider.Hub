using DotnetSpider.Enterprise.Application.Log.Dto;
using DotnetSpider.Enterprise.Application.Task.Dtos;
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
		protected readonly ILogger _logger;

		public LogAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession,
			UserManager<Domain.Entities.ApplicationUser> userManager, ILogger<LogAppService> logger)
			: base(dbcontext, configuration, appSession, userManager)
		{
			_logger = logger;
		}

		public async void Sumit(LogInputDto input)
		{
			try
			{
				var client = new MongoClient(Configuration.LogMongoConnectionString);
				var database = client.GetDatabase("dotnetspider");
				var collection = database.GetCollection<BsonDocument>(input.Identity);

				await collection.InsertOneAsync(BsonDocument.Parse(input.LogInfo.ToString()));
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Submit log failed.");
			}
		}

		public PagingLogOutDto Query(PagingLogInputDto input)
		{
			var client = new MongoClient(Configuration.LogMongoConnectionString);
			var database = client.GetDatabase("dotnetspider");
			var collection = database.GetCollection<BsonDocument>(input.Identity);

			var result = new PagingLogOutDto
			{
				Page = input.Page,
				Size = input.Size,
				Columns = new List<string>(),
				Values = new List<List<string>>()
			};

			List<BsonDocument> list = null;
			if (!string.IsNullOrEmpty(input.NodeId))
			{
				list = collection.Find(new BsonDocument("Node", input.NodeId)).Skip((input.Page - 1) * input.Size).Limit(input.Size).Sort(Builders<BsonDocument>.Sort.Descending("_id")).ToList();
				result.Total = collection.Find(new BsonDocument("Node", input.NodeId)).Count();
			}
			else
			{
				list = collection.Find(new BsonDocument()).Skip((input.Page - 1) * input.Size).Limit(input.Size).Sort(Builders<BsonDocument>.Sort.Descending("_id")).ToList();
				result.Total = collection.Find(new BsonDocument()).Count();
			}

			if (list.Count > 0)
			{
				var head = list.First();
				foreach (var hi in head.Elements)
				{
					if (hi.Name == "_id")
					{
						continue;
					}
					result.Columns.Add(hi.Name);
				}
				foreach (var item in list)
				{
					var vlist = new List<string>();
					foreach (var v in item.Elements.Where(a => a.Name != "_id"))
					{
						vlist.Add(v.Value is BsonNull ? string.Empty : v.Value.ToString());
					}
					result.Values.Add(vlist);
				}
			}
			return result;
		}
	}
}
