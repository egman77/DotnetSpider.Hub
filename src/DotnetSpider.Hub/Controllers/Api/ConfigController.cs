using DotnetSpider.Hub.Application.Config;
using DotnetSpider.Hub.Core.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace DotnetSpider.Hub.Controllers.Api
{
	[Route("api/v1.0/[controller]")]
	public class ConfigController : BaseController
	{
		private readonly IConfigAppService _configAppService;

		public ConfigController(ICommonConfiguration commonConfiguration,
			IConfigAppService configAppService) : base(commonConfiguration)
		{
			_configAppService = configAppService;
		}

		public string Get(string name)
		{
			CheckAuth(true);
			return _configAppService.Get(name);
		}
	}
}
