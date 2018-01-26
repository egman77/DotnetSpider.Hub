using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ActionType
	{
		Disable,
		Enable,
		Exit,
		Run,
		Increase,
		Reduce
	}
}
