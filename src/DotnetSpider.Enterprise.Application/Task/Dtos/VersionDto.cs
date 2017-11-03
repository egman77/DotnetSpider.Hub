using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class VersionDto
	{
		public virtual string Version { get; set; }

		public virtual DateTime PublishTime { get; set; }

		public virtual string Tags { get; set; }
	}
}
