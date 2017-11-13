using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Application.TaskStatus.Dtos
{
	public class TaskStatusDto
	{
		public virtual string Name { get; set; }
		public virtual string Identity { get; set; }
		public virtual string NodeId { get; set; }
		public virtual string Status { get; set; }
		public virtual int Thread { get; set; }
		public virtual long Left { get; set; }
		public virtual long Success { get; set; }
		public virtual long Error { get; set; }
		public virtual long Total { get; set; }
		public virtual float AvgDownloadSpeed { get; set; }
		public virtual float AvgProcessorSpeed { get; set; }
		public virtual float AvgPipelineSpeed { get; set; }
		public virtual DateTime? LastModificationTime { get; set; }
		public virtual long TaskId { get; set; }
	}
}
