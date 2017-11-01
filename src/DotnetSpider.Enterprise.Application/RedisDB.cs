using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DotnetSpider.Enterprise.Application
{
	public class AppRedisDb
	{
		public IDatabase RedisDb { get; set; }
		public string SystemName { get; set; }
		public string ServiceName { get; set; }

		public AppRedisDb(string serviceName, IDatabase db)
		{
			SystemName = "DotnetSpider.Enterprise";
			ServiceName = serviceName;
			RedisDb = db;
		}

		public AppRedisDb(string systemName, string serviceName, IDatabase db)
		{
			SystemName = systemName;
			ServiceName = serviceName;
			RedisDb = db;
		}

		public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expire = null)
		{
			var redisKey = GenerateRealKey(key);
			return await RedisDb.StringSetAsync(redisKey, value, expire);
		}

		public bool StringSet(string key, string value, TimeSpan? expire = null)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.StringSet(redisKey, value, expire);
		}

		public RedisValue StringGet(string key)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.StringGet(redisKey);
		}

		public RedisValue[] StringGet(string[] keys)
		{
			var redisKeys = GenerateRealKeys(keys);
			return RedisDb.StringGet(redisKeys);
		}

		public RedisValue StringGetSet(string key, string value)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.StringGetSet(redisKey, value);
		}

		public bool KeyDelete(string key)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.KeyDelete(redisKey);
		}

		public HashEntry[] HashGetAll(string key)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.HashGetAll(redisKey);
		}

		public bool HashSet(string key, string field, string value)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.HashSet(redisKey, field, value);
		}

		public RedisValue HashGet(string key, string field)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.HashGet(redisKey, field);
		}


		public bool HashDelete(string key, string field)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.HashDelete(redisKey, field);
		}

		public long ListRemove(string key, string value)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.ListRemove(redisKey, value);
		}

		public long ListRightPush(string key, string value)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.ListRightPush(redisKey, value);
		}

		public RedisValue ListLeftPop(string key)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.ListLeftPop(redisKey);
		}

		public bool KeyExists(string key)
		{
			var redisKey = GenerateRealKey(key);
			return RedisDb.KeyExists(redisKey);
		}

		private string GenerateRealKey(string key)
		{
			return $"{SystemName}:{ServiceName}:{key}";
		}

		private RedisKey[] GenerateRealKeys(string[] keys)
		{
			var redisKeys = new RedisKey[keys.Length];

			for (var i = 0; i < keys.Length; i++)
			{
				redisKeys[i] = GenerateRealKey(keys[i]);
			}
			return redisKeys;
		}
	}
}
