using System.ComponentModel.DataAnnotations;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class Project : AuditedEntity<int>
	{
		[Required]
		[StringLength(100)]
		public virtual string Name { get; set; }

		[Required]
		[StringLength(300)]
		public virtual string GitUrl { get; set; }

		[Required]
		[StringLength(20)]
		public virtual string Framework { get; set; }

		[StringLength(200)]
		public virtual string IntervalPath { get; set; }

		public virtual bool IsEnabled { get; set; }

		[StringLength(100)]
		public virtual string EntryProject { get; set; }
	}
}
