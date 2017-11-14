using DotnetSpider.Enterprise.Application.TaskStatus.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskHistory.Dtos
{
	public class TaskHistoryOutputDto
	{
		public long TaskId { get; set; }

		public string Identity { get; set; }

		public List<TaskStatusOutputDto> Statuses { get; set; }
	}
}
