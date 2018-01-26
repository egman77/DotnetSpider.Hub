using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotnetSpider.Enterprise.Application.Node.Dto
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ActionType
	{
		Disable,
		Enable,
		Exit
	}
}
