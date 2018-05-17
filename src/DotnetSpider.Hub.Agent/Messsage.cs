using System;

namespace DotnetSpider.Hub.Agent
{
	public class Messsage
	{
		public virtual string NodeId { get; set; }
		public virtual string Name { get; set; }
		public virtual string Arguments { get; set; }
		public virtual string ApplicationName { get; set; }
		public virtual string Version { get; set; }
		public virtual long TaskId { get; set; }
		public virtual DateTime CreationTime { get; set; }
	}
}