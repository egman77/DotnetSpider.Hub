using System.Collections.Generic;
using DotnetSpider.Hub.Application.TaskStatus.Dtos;

namespace DotnetSpider.Hub.Application.TaskHistory.Dtos
{
	public class TaskHistoryOutput
	{
		public string TaskId { get; set; }

		public string Identity { get; set; }

		public string CreationTime { get; set; }

		public List<TaskStatusDto> Statuses { get; set; }
	}
}
