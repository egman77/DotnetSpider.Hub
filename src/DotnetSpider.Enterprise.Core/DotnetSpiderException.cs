using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Core
{
	public class DotnetSpiderException : Exception
	{
		public DotnetSpiderException(string message) : base(message)
		{
		}
	}
}
