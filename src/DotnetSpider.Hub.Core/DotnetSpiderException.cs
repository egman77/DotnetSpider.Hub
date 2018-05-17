using System;

namespace DotnetSpider.Hub.Core
{
	public class DotnetSpiderException : Exception
	{
		public DotnetSpiderException(string message) : base(message)
		{
		}
	}
}
