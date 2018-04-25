using System;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Core.Entities
{
	public class FilterQueryInput
	{
		private Dictionary<string, string> _dictionary;

		public virtual string Filter { get; set; }

		public IDictionary<string, string> ToDictionary()
		{
			if (_dictionary == null)
			{
				_dictionary = new Dictionary<string, string>();

				foreach (var kvs in Filter.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
				{
					var kv = kvs.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
					var key = kv[0].ToLower();
					var value = kv.Length > 1 ? kv[1] : null;
					if (_dictionary.ContainsKey(key))
					{
						_dictionary[key] = value;
					}
					else
					{
						_dictionary.Add(key, value);
					}
				}
			}
			return _dictionary;
		}

		public string GetFilterValue(string filter)
		{
			string value;
			ToDictionary().TryGetValue(filter, out value);
			return value;
		}
	}
}
