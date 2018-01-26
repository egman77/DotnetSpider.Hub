using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

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
