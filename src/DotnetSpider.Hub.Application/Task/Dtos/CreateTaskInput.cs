﻿using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Application.Task.Dtos
{
	public class CreateTaskInput
	{
		/// <summary>
		/// 程序集名称
		/// </summary>
		[StringLength(100)]
		[Required]
		public virtual string ApplicationName { get; set; }

		/// <summary>
		/// Cron表达式 (计划任务--cron)
		/// </summary>
		[StringLength(50)]
		[Required]
		public virtual string Cron { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		[StringLength(500)]
		public virtual string Arguments { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool IsEnabled { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		[Required]
		[StringLength(50)]
		public virtual string Name { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		[Required]
		public virtual int NodeCount { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		public virtual int NodeRunningCount { get; set; }

		public virtual string NodeType { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		public virtual string Description { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		[StringLength(100)]
		public virtual string Owners { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		public virtual string Developers { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		[StringLength(100)]
		public virtual string Analysts { get; set; }

		/// <summary>
		/// 版本信息
		/// </summary>
		[StringLength(100)]
		public virtual string Package { get; set; }

		public virtual bool IsSingle { get; set; }

		public virtual bool IsDelete { get; set; }

		[StringLength(20)]
		public virtual string Os { get; set; }

		public virtual string Tags { get; set; }

		[StringLength(32)]
		public virtual string LastIdentity { get; set; }
	}
}
