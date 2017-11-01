using DotnetSpider.Enterprise.Domain.Entities;

namespace DotnetSpider.Enterprise.Application.User
{
	public interface IUserAppService
	{
		ApplicationUser GetUserById(long userId);
		ApplicationUser GetUserByName(string userName);
		ApplicationUser GetUserByNameOrEmailAddress(string userNameOrEmailAddress);
	}
}