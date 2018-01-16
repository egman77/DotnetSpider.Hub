using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class TaskIdInput
	{
		[Required]
		public virtual long TaskId { get; set; }
	}
}
