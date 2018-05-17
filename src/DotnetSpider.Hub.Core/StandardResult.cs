using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotnetSpider.Hub.Core
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
