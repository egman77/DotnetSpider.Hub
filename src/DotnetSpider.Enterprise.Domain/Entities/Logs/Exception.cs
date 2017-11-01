using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpider.Enterprise.Domain.Entities.Logs
{
	public class Exception : Entity<long>
	{
		public long? UserId { get; set; }
		public string QueryString { get; set; }
		public string Path { get; set; }
		public string Message { get; set; }
		public DateTime CreationTime { get; set; }
		public string SessionId { get; set; }
	}
}
