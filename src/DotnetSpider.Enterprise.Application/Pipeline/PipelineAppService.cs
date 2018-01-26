using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using DotnetSpider.Enterprise.Core.Configuration;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using MessagePack;
using Microsoft.Extensions.Logging;
using DotnetSpider.Enterprise.Core;

namespace DotnetSpider.Enterprise.Application.Pipeline
{
	public class PipelineAppService : AppServiceBase, IPipelineAppService
	{
		public class HttpPipelinePackage
		{
			/// <summary>
			/// Sql
			/// </summary>
			public string Sql { get; set; }

			public Database D { get; set; }

			public List<Dictionary<string, object>> Dt { get; set; }
		}

		[Flags]
		public enum Database
		{
			MySql,
			SqlServer,
			MongoDb,
			Cassandra,
			PostgreSql
		}

		private readonly ICryptoTransform _cryptoTransform;

		public PipelineAppService(ApplicationDbContext dbcontext, ICommonConfiguration configuration, IAppSession appSession, UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory)
			: base(dbcontext, configuration, appSession, userManager, loggerFactory)
		{
			var des = DES.Create();
			_cryptoTransform = des.CreateDecryptor(configuration.SqlEncryptKey, configuration.SqlEncryptKey);
		}

		public int Process(Stream content)
		{
			var memory = new MemoryStream();
			content.CopyTo(memory);

			int result = -100;
			var bytes = memory.ToArray();
			var json = Encoding.UTF8.GetString(LZ4MessagePackSerializer.Decode(bytes));
			if (string.IsNullOrEmpty(json) || string.IsNullOrWhiteSpace(json))
			{
				return result;
			}
			var resultStream = new MemoryStream();
			var package = JsonConvert.DeserializeObject<HttpPipelinePackage>(json);
			var cryptorSql = package.Sql;
			var sqlBytes = Convert.FromBase64String(cryptorSql);

			MemoryStream ms = new MemoryStream(sqlBytes);
			CryptoStream cst = new CryptoStream(ms, _cryptoTransform, CryptoStreamMode.Read);

			using (StreamReader sr = new StreamReader(cst))
			{
				var sql = sr.ReadToEnd();

				try
				{
					switch (package.D)
					{
						case Database.MySql:
							{
								using (var conn = new MySqlConnection(Configuration.MySqlConnectionString))
								{
									if (package.Dt == null)
									{
										result = conn.Execute(sql);
									}
									else
									{
										result = conn.Execute(sql, package.Dt);
									}
								}
								break;
							}
						default:
							{
								result = -100;
								break;
							}
					}
				}
				catch (Exception e)
				{
					Logger.LogError($"Pipeline execute failed {json}: {e}");
					throw e;
				}
			}

			return result;
		}
	}
}
