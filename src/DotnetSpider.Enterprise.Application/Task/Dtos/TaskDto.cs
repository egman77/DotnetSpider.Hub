using System;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class TaskDto
	{
		public long Id { get; set; }
		/// <summary>
		/// 程序集名称
		/// </summary>
		public virtual string ApplicationName { get; set; }

		/// <summary>
		/// Cron表达式
		/// </summary>
		public virtual string Cron { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		public virtual string Arguments { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool IsEnabled { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		public virtual int NodeCount { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		public virtual int NodeRunningCount { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		public virtual string Description { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		public virtual string Owners { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		public virtual string Developers { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		public virtual string Analysts { get; set; }

		/// <summary>
		/// 版本信息
		/// </summary>
		public virtual string Version { get; set; }


		/// <summary>
		/// 项目ID
		/// </summary>
		public virtual long ProjectId { get; set; }

		public virtual bool IsDelete { get; set; }

		public virtual string Os { get; set; }

		public virtual string Tags { get; set; }
	}

	public class RunningTaskDto: TaskDto
	{
		public virtual string Identity { get; set; }
		public virtual DateTime CDate { get; set; }
	}
}
