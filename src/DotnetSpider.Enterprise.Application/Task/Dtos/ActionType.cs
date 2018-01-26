using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

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
