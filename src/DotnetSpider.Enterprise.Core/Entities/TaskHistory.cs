using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Core.Entities
{
	public class TaskHistory : AuditedEntity<long>
	{
		[Required]
		public virtual long TaskId { get; set; }

		[Required]
		[StringLength(32)]
		public virtual string Identity { get; set; }

		public virtual string NodeIds { get; set; }
	}
}
