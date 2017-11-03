using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Project.Dtos
{
	public class StatusDto
	{
		public long BuildId { get; set; }
		public string Status { get; set; }
		public string FilePath { get; set; }
		
		public List<BuildLogDto> Logs { get; set; }
	}

	public class BuildLogDto
	{
		public string LogType { get; set; }
		public DateTime CreationTime { get; set; }
		public string Message { get; set; }
	}
}
