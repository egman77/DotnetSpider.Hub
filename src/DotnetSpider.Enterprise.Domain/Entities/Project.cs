using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Project : AuditedEntity<long>
	{
		/// <summary>
		/// 项目名称
		/// </summary>
		[Required]
		[StringLength(100)]
		public virtual string Name { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public virtual bool IsEnabled { get; set; }

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
		/// 备注信息
		/// </summary>
		[StringLength(500)]
		public virtual string Note { get; set; }
	}
}
