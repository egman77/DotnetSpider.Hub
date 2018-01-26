using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Core
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Status
	{
		Success,
		Error,
		Failed
	}

	public class StandardResult
	{
		public int Code { get; set; }
		public Status Status { get; set; }
		public string Message { get; set; }
		public object Data { get; set; }
	}
}
