using DotnetSpider.Enterprise.Application.Task.Dtos;
using DotnetSpider.Enterprise.Domain;
using DotnetSpider.Enterprise.Domain.Entities;
using System.Collections.Generic;

namespace DotnetSpider.Enterprise.Application.System
{
	public interface ISystemAppService
	{
		void Register();
		void Execute(string name, string arguments);
	}
}
