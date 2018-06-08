using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotnetSpider.Hub.Application.Task.Dtos
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ActionType
	{
		Query,
		Disable,
		Enable,
		Exit,
		Run,
		Increase,
		Reduce
	}
}
