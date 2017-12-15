using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Message : AuditedEntity<long>
	{
		public const string RunMessageName = "RUN";
		public const string CanleMessageName = "CANCEL";
		public const string ExitMessageName = "EXIT";

		[Required]
		[StringLength(32)]
		public virtual string NodeId { get; set; }

		[Required]
		public virtual long TaskId { get; set; }

		[StringLength(100)]
		public virtual string Name { get; set; }

		[StringLength(100)]
		[Required]
		public virtual string ApplicationName { get; set; }

		/// <summary>
		/// 版本信息
		/// </summary>
		[StringLength(100)]
		public virtual string Version { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		[StringLength(500)]
		public virtual string Arguments { get; set; }
	}
}
