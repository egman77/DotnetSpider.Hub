using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskRunning.Dtos
{
	public class AddTaskRunningInputDto
	{
		public virtual long TaskId { get; set; }

		public virtual string Identity { get; set; }
	}
}
