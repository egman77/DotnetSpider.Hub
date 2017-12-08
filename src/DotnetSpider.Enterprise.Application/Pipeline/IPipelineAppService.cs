using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotnetSpider.Enterprise.Application.Pipeline
{
	public interface IPipelineAppService
	{
		int Process(Stream content);
	}
}
