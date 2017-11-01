using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities
{
	public class ApplicationUser : IdentityUser<long>, IAuditedEntity
	{
		public virtual DateTime? LastModificationTime { get; set; }
		public virtual long? LastModifierUserId { get; set; }
		public virtual DateTime CreationTime { get; set; }
		public virtual long? CreatorUserId { get; set; }
		public virtual bool IsActive { get; set; }
	}
}
