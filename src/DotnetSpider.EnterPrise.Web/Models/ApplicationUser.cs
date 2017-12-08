using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DotnetSpider.EnterPrise.Web.Models
{
	// Add profile data for application users by adding properties to the ApplicationUser class
	public class ApplicationUser : IdentityUser<long>
	{
	}

	public class ApplicationRole : IdentityRole<long>
	{
	}

	public class ApplicationRoleClaim : IdentityRoleClaim<long>
	{
	}

	public class ApplicationUserClaim : IdentityUserClaim<long>
	{
	}

	public class ApplicationUserLogin : IdentityUserLogin<long>
	{
	}

	public class ApplicationUserRole : IdentityUserRole<long>
	{
	}

	public class ApplicationUserToken : IdentityUserToken<long>
	{
	}
}
