namespace DotnetSpider.Hub.Agent.Process
{
	public class ProcessInfo
	{
		public virtual string TaskId { get; set; }
		public virtual System.Diagnostics.Process Process { get; set; }
		public virtual string WorkingDirectory { get; set; }
	}
}
