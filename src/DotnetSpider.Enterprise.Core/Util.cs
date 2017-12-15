using System.Net;
using System.Net.Http;

namespace DotnetSpider.Enterprise.Core
{
	public static class Util
	{
		public static HttpClient Client = new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseProxy = true,
			UseCookies = false
		});
	}
}
