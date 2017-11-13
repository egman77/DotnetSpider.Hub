using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DotnetSpider.Enterprise.Core
{
	public static class Util
	{
		public static HttpClient Client = new HttpClient(new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseCookies = false
		});
	}
}
