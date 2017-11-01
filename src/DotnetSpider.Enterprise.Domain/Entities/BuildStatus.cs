using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class BuildStatus : Entity<long>
	{
		public virtual long SolutionId { get; set; }

		[StringLength(32)]
		public virtual string Status { get; set; }
		public virtual bool Succeeded { get; set; }
		[StringLength(500)]
		public virtual string FilePath { get; set; }
		public virtual DateTime CreationTime { get; set; }
	}
}
