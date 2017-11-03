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
		/// 任务名称
		/// </summary>
		[Required]
		[StringLength(100)]
		public virtual string Name { get; set; }

		/// <summary>
		/// Framework类型
		/// </summary>
		[StringLength(20)]
		public virtual string Framework { get; set; }

		/// <summary>
		/// 程序集名称
		/// </summary>
		[StringLength(100)]
		public virtual string AssemblyName { get; set; }

		/// <summary>
		/// 爬虫名称或完全限定名 -s参数
		/// </summary>
		[StringLength(100)]
		public virtual string TaskName { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		[StringLength(100)]
		public virtual string ExtraArguments { get; set; }

		/// <summary>
		/// 所需节点数
		/// </summary>
		[Required]
		public virtual int NodesCount { get; set; }

		/// <summary>
		/// Cron表达式，以|分割
		/// </summary>
		[StringLength(100)]
		public virtual string Cron { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool IsEnabled { get; set; }

		/// <summary>
		/// 版本信息（Git）
		/// </summary>
		[StringLength(100)]
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
		[StringLength(50)]
		public virtual string Client { get; set; }

		/// <summary>
		/// 管理人
		/// </summary>
		[StringLength(50)]
		public virtual string Executive { get; set; }

		/// <summary>
		/// 开发者
		/// </summary>
		[StringLength(50)]
		public virtual string Programmer { get; set; }

		/// <summary>
		/// 备注信息
		/// </summary>
		[StringLength(500)]
		public virtual string Note { get; set; }

		[ForeignKey("ProjectId")]
		public virtual Project Project { get; set; }
	}
}
