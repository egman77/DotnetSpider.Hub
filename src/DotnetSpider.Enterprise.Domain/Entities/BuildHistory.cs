using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class BuildHistory : Entity<long>
	{
		public virtual long SolutionId { get; set; }
		public virtual long BuildId { get; set; }

		[StringLength(50)]
		public virtual string GitVersion { get; set; }

		public virtual DateTime PublishTime { get; set; }

		public virtual string Spiders { get; set; }

		[StringLength(50)]
		public virtual string Tags { get; set; }
	}
}
