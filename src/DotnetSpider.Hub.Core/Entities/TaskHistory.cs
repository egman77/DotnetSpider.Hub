using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Hub.Core.Entities
{
	public class TaskHistory : AuditedEntity
	{
		[Required]
		public virtual string TaskId { get; set; }

		[Required]
		[StringLength(32)]
		public virtual string Identity { get; set; }

		public virtual string NodeIds { get; set; }
	}
}
