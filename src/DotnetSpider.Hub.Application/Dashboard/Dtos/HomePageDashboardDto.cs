using System.Collections.Generic;
using DotnetSpider.Hub.Application.Node.Dtos;

namespace DotnetSpider.Hub.Application.Report.Dtos
{
	public class HomePageDashboardDto
	{
		public virtual int TaskCount { get; set; }
		public virtual string LogSize { get; set; }
		public virtual int NodeTotalCount { get; set; }
		public virtual int NodeOnlineCount { get; set; }
		public virtual int RunningTaskCount { get; set; }
		public virtual List<NodeDto> Nodes { get; set; }
	}
}
