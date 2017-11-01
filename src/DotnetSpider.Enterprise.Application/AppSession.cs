using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace DotnetSpider.Enterprise.Application
{
	public class AppSession : IAppSession
	{
		private readonly IHttpContextAccessor _accessor;
		//private UserQuota _userQuota;
		//private readonly IUserAppService _userAppService;
		private readonly ApplicationDbContext _dbContext;

		public AppSession(IHttpContextAccessor accessor, ApplicationDbContext dbContext)
		{
			_accessor = accessor;
			_dbContext = dbContext;
		}

	
		public long? UserId
		{
			get
			{
				var value = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				long userId;
				if (long.TryParse(value, out userId))
				{
					return userId;
				}
				return null;
			}

		}

		public ClaimsPrincipal ClaimsPrincipal
		{
			get
			{
				return _accessor.HttpContext.User;
			}
		}


		public HttpRequest GetRequest()
		{
			return _accessor.HttpContext.Request;
		}

		public long GetUserId()
		{
			var value = _accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			long userId;
			if (long.TryParse(value, out userId))
			{
				return userId;
			}
			throw new Exception("BadRequest");
		}
	}
}
