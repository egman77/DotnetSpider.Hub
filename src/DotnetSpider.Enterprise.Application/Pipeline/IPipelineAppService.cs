using System.IO;

namespace DotnetSpider.Enterprise.Application.Pipeline
{
	public interface IPipelineAppService
	{
		int Process(Stream content);
	}
}
