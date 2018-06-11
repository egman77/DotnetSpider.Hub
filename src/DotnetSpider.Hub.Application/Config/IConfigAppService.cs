using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Hub.Application.Config
{
	public interface IConfigAppService
	{
		string Get(string name);
	}
}
