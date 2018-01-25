namespace DotnetSpider.Enterprise.Application.Scheduler.Dtos
{
	public class SchedulerJobDto
	{
		public virtual string Id { get; set; }

		public virtual string Name { get; set; }

		public virtual string Cron { get; set; }

		public virtual string Url { get; set; }

		public virtual string Data { get; set; }
	}
}
