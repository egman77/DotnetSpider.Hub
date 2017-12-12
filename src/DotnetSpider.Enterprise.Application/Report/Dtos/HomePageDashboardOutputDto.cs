using DotnetSpider.Enterprise.Application.Node.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Report.Dtos
{
	public class HomePageDashboardOutputDto
	{
		public virtual int TaskCount { get; set; }
		public virtual string LogSize { get; set; }
		public virtual int NodeTotalCount { get; set; }
		public virtual int NodeOnlineCount { get; set; }
		public virtual int RunningTaskCount { get; set; }
		public virtual List<NodeOutputDto> Nodes { get; set; }
	}
}
