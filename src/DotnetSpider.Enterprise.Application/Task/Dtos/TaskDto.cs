using System;

namespace DotnetSpider.Enterprise.Application.Task.Dtos
{
	public class TaskDto
	{
		public virtual long Id { get; set; }
		/// <summary>
		/// 任务名称
		/// </summary>
		public virtual string Name { get; set; }

		/// <summary>
		/// Framework类型
		/// </summary>
		public virtual string Framework { get; set; }

		/// <summary>
		/// 程序集名称
		/// </summary>
		public virtual string AssemblyName { get; set; }

		/// <summary>
		/// 爬虫名称或完全限定名 -s参数
		/// </summary>
		public virtual string TaskName { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		public virtual string ExtraArguments { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		public virtual int NodesCount { get; set; }

		/// <summary>
		/// Cron表达式，以|分割
		/// </summary>
		public virtual string Cron { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool IsEnabled { get; set; }

		/// <summary>
		/// 版本信息（Git）
		/// </summary>
		public virtual string Version { get; set; }

		/// <summary>
		/// 编译时间
		/// </summary>
		public virtual DateTime BuildTime { get; set; }

		/// <summary>
		/// 项目ID
		/// </summary>
		public virtual int ProjectId { get; set; }

		/// <summary>
		/// 委托方，客户
		/// </summary>
		public virtual string Client { get; set; }

		/// <summary>
		/// 管理人
		/// </summary>
		public virtual string Executive { get; set; }

		/// <summary>
		/// 开发者
		/// </summary>
		public virtual string Programmer { get; set; }

		/// <summary>
		/// 备注信息
		/// </summary>
		public virtual string Note { get; set; }
	}

	public class RunningTaskDto: TaskDto
	{
		public virtual string Identity { get; set; }
		public virtual DateTime CDate { get; set; }
	}
}
