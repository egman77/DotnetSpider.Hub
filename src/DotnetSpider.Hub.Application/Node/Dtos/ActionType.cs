using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotnetSpider.Hub.Application.Node.Dtos
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ActionType
	{
		Disable,
		Enable,
		Exit
	}
}
