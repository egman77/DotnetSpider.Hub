using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Task : AuditedEntity<long>
	{
		/// <summary>
		/// 程序集名称
		/// </summary>
		[StringLength(100)]
		[Required]
		public virtual string ApplicationName { get; set; }

		/// <summary>
		/// Cron表达式
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

		/// <summary>
		/// 1 为公司内部节点  2为VPS节点
		/// </summary>
		public virtual int NodeType { get; set; }

		[StringLength(32)]
		public virtual string LastIdentity { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		[StringLength(500)]
		public virtual string Description { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		[StringLength(100)]
		public virtual string Owners { get; set; }

		/// <summary>
		/// 任务名称
		/// </summary>
		[StringLength(100)]
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
		public virtual string Version { get; set; }

		public virtual bool IsSingle { get; set; } = true;

		public virtual bool IsDeleted { get; set; }

		[StringLength(20)]
		public virtual string Os { get; set; }

		[StringLength(100)]
		public virtual string Tags { get; set; }

		public virtual bool IsRunning { get; set; }
	}
}
