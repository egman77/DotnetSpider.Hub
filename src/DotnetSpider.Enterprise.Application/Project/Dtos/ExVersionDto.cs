using DotnetSpider.Enterprise.Application.Task.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Project.Dtos
{
	public class ExVersionDto : VersionDto
	{
		public virtual string[] SpiderNames { get; set; }
	}
}
