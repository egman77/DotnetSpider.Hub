using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DotnetSpider.Enterprise.Agent
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
		public JToken Data { get; set; }
	}
}
