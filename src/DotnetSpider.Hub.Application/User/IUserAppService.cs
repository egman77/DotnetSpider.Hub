using DotnetSpider.Hub.Core.Entities;

namespace DotnetSpider.Hub.Application.User
{
	public interface IUserAppService
	{
		ApplicationUser GetUserById(long userId);
		ApplicationUser GetUserByName(string userName);
		ApplicationUser GetUserByNameOrEmailAddress(string userNameOrEmailAddress);
	}
}