using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.Enterprise.Core.Entities
{
	public class ApplicationUser : IdentityUser<long>, IAuditedEntity
	{
		public virtual DateTime? LastModificationTime { get; set; }
		public virtual long? LastModifierUserId { get; set; }
		public virtual DateTime CreationTime { get; set; }
		public virtual long? CreatorUserId { get; set; }
		public virtual bool IsActive { get; set; }

		/// <summary>
		/// Navigation property for this users login accounts.
		/// </summary>
		public virtual ICollection<ApplicationUserLogin> Logins { get; } = new List<ApplicationUserLogin>();

		public virtual ICollection<ApplicationUserRole> Roles { get; } = new List<ApplicationUserRole>();

		public virtual ICollection<ApplicationUserClaim> Claims { get; } = new List<ApplicationUserClaim>();
	}

	public class ApplicationRole : IdentityRole<long>
	{
		public virtual ICollection<ApplicationUserClaim> Claims { get; set; } = new List<ApplicationUserClaim>();
		public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

		public ApplicationRole()
		{
		}

		public ApplicationRole(string roleName)
		{

		}
		public ApplicationRole(string roleName, string description)
		{
			Description = description;
		}

		public string Description { get; set; }
	}

	public class ApplicationUserLogin : IdentityUserLogin<long>
	{
		public virtual ApplicationUser ApplicationUser { get; set; }
	}

	public class ApplicationUserRole : IdentityUserRole<long>
	{
		public virtual ApplicationUser ApplicationUser { get; set; }
	}

	public class ApplicationUserClaim : IdentityUserClaim<long>
	{
		public virtual ApplicationUser ApplicationUser { get; set; }
	}

	public class ApplicationRoleClaim : IdentityRoleClaim<long>
	{
	}

	public class ApplicationUserToken : IdentityUserToken<long>
	{
	}
}
