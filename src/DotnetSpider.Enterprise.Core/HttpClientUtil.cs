using System.Net;
using System.Net.Http;

namespace DotnetSpider.Enterprise.Core
{
	public static class HttpClientUtil
	{
		public static HttpClient DefaultClient = new HttpClient(new HttpClientHandler
		{
			AllowAutoRedirect = true,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseProxy = true,
			UseCookies = false
		});
	}
}
